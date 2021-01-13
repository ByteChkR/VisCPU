﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using VisCPU.HL.Compiler;
using VisCPU.HL.DataTypes;
using VisCPU.HL.Events;
using VisCPU.HL.Importer;
using VisCPU.HL.Importer.Events;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Events;
using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.Parser.Tokens.Combined;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.IO;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;
using VisCPU.Utility.UriResolvers;

namespace VisCPU.HL
{

    public class HLCompilation : VisBase
    {

        public readonly HLTypeSystem TypeSystem = new HLTypeSystem();
        internal const string CONST_VAL_TYPE = "const_var";
        internal const string VAL_TYPE = "var";
        internal readonly Dictionary < string, string > ConstValTypes = new Dictionary < string, string >();
        internal readonly List < IExternalData > ExternalSymbols = new List < IExternalData >();
        internal readonly Dictionary < string, FunctionData > FunctionMap = new Dictionary < string, FunctionData >();

        internal readonly string OriginalText;
        internal readonly List < string > ProgramCode = new List < string >();

        internal HLCompilerCollection TypeMap;

        private static uint counter;
        private readonly string Directory;

        private readonly List < string > IncludedFiles = new List < string >();
        private readonly List < string > ImportedItems = new List < string >();
        private readonly HLCompilation parent;
        private readonly string scopeID = "_";

        private readonly Queue < string > unusedTempVars = new Queue < string >();
        private readonly List < string > usedTempVars = new List < string >();
        private readonly Dictionary < string, VariableData > VariableMap = new Dictionary < string, VariableData >();
        private string ParsedText;

        private BuildDataStore dataStore;

        public event Action < string, string > OnCompiledIncludedScript;

        protected override LoggerSystems SubSystem => LoggerSystems.HL_Compiler;

        #region Public

        public HLCompilation( string originalText, string directory ) : this(
                                                                             originalText,
                                                                             directory,
                                                                             new BuildDataStore(
                                                                                  directory,
                                                                                  new HLBuildDataStore()
                                                                                 )
                                                                            )
        {
        }

        public HLCompilation( string originalText, string directory, BuildDataStore dataStore )
        {
            this.dataStore = dataStore;
            OriginalText = originalText;
            Directory = directory;
            TypeMap = new HLCompilerCollection( TypeSystem );
        }

        public HLCompilation( HLCompilation parent, string id )
        {
            dataStore = parent.dataStore;
            scopeID = id;
            OriginalText = parent.OriginalText;
            Directory = parent.Directory;
            VariableMap = new Dictionary < string, VariableData >( parent.VariableMap );
            ConstValTypes = new Dictionary < string, string >( parent.ConstValTypes );
            FunctionMap = new Dictionary < string, FunctionData >( parent.FunctionMap );
            ExternalSymbols = new List < IExternalData >( parent.ExternalSymbols );
            IncludedFiles = new List < string >( parent.IncludedFiles );
            TypeSystem = parent.TypeSystem;
            this.parent = parent;
            TypeMap = new HLCompilerCollection( TypeSystem );
        }

        public static void ResetCounter()
        {
            counter = 0;
        }

        public bool ContainsLocalVariable( string var )
        {
            return VariableMap.ContainsKey( GetFinalName( var ) );
        }

        public bool ContainsVariable( string var )
        {
            return VariableMap.ContainsKey( GetFinalName( var ) ) || parent != null && parent.ContainsVariable( var );
        }

        public void CreateVariable( string name, uint size, HLTypeDefinition tdef, bool isVisible )
        {
            VariableMap[GetFinalName( name )] = new VariableData( name, GetFinalName( name ), size, tdef, isVisible );
        }

        public void CreateVariable( string name, string content, HLTypeDefinition tdef, bool isVisible )
        {
            VariableMap[GetFinalName( name )] =
                new VariableData( name, GetFinalName( name ), content, tdef, isVisible );
        }

        public string GetFinalName( string name )
        {
            return GetPrefix() + name;
        }

        public VariableData GetVariable( string name )
        {
            if ( VariableMap.ContainsKey( GetFinalName( name ) ) )
            {
                return VariableMap[GetFinalName( name )];
            }

            if ( parent == null )
            {
                EventManager < ErrorEvent >.SendEvent( new HLVariableNotFoundEvent( name, false ) );

                return new VariableData();
            }

            return parent.GetVariable( name );
        }

        public string Parse( bool printHead = true, string appendAfterProg = "HLT" )
        {
            if ( ParsedText != null )
            {
                return ParsedText;
            }

            HLParserSettings hlpS = new HLParserSettings();
            HLParserBaseReader br = new HLParserBaseReader( hlpS, OriginalText );

            List < IHLToken > tokens = br.ReadToEnd();
            ParseOneLineStrings( tokens );
            ParseCharTokens( tokens );
            RemoveComments( tokens );
            ParseImports( tokens );
            ParseIncludes( tokens );
            ParseReservedKeys( tokens, hlpS );
            tokens = tokens.Where( x => x.Type != HLTokenType.OpNewLine ).ToList();
            ParseVarDefToken( tokens, hlpS );
            ParseBlocks( tokens );

            ParseFunctionToken( tokens, hlpS );
            ParseTypeDefinitions( TypeSystem, hlpS, tokens );

            HLExpressionParser p = HLExpressionParser.Create( new HLExpressionReader( tokens ) );
            ProcessImports();
            ParseDependencies();

            return Parse( p.Parse(), printHead, appendAfterProg );
        }

        public void ParseBlocks( List < IHLToken > tokens )
        {
            for ( int i = tokens.Count - 1; i >= 0; i-- )
            {
                if ( tokens[i].Type == HLTokenType.OpBlockBracketClose )
                {
                    int current = 1;
                    int start = i - 1;

                    for ( ; start >= 0; start-- )
                    {
                        if ( tokens[start].Type == HLTokenType.OpBlockBracketClose )
                        {
                            current++;
                        }
                        else if ( tokens[start].Type == HLTokenType.OpBlockBracketOpen )
                        {
                            current--;

                            if ( current == 0 )
                            {
                                break;
                            }
                        }
                    }

                    List < IHLToken > content = tokens.GetRange( start + 1, i - start - 1 ).ToList();
                    tokens.RemoveRange( start, i - start + 1 );
                    ParseBlocks( content );
                    tokens.Insert( start, new BlockToken( content.ToArray(), start ) );
                    i = start;
                }
            }
        }

        public void ParseCharTokens( List < IHLToken > tokens )
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                if ( i < tokens.Count - 1 &&
                     tokens[i].Type == HLTokenType.OpSingleQuote )
                {
                    int idx = tokens.FindIndex( i + 1, t => t.Type == HLTokenType.OpNewLine );
                    int endQuote = tokens.FindIndex( i + 1, t => t.Type == HLTokenType.OpSingleQuote );

                    if ( idx == -1 )
                    {
                        idx = tokens.Count - 1;
                    }

                    if ( endQuote == -1 || endQuote > idx )
                    {
                        EventManager < ErrorEvent >.SendEvent(
                                                              new HLTokenReadEvent(
                                                                   HLTokenType.OpSingleQuote,
                                                                   HLTokenType.OpNewLine
                                                                  )
                                                             );

                        return;
                    }

                    List < IHLToken > content = tokens.GetRange( i + 1, endQuote - i - 1 );

                    string ConcatContent()
                    {
                        return OriginalText.Substring(
                                                      content.First().SourceIndex,
                                                      tokens[i + 1 + content.Count].SourceIndex -
                                                      content.First().SourceIndex
                                                     );
                    }

                    IHLToken newToken = new HLTextToken(
                                                        HLTokenType.OpCharLiteral,
                                                        ConcatContent(),
                                                        tokens[i].SourceIndex
                                                       );

                    tokens.RemoveRange( i, endQuote - i + 1 );
                    tokens.Insert( i, newToken );
                }
            }
        }

        public void ParseFunctionToken( List < IHLToken > tokens, HLParserSettings settings )
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                if ( tokens[i].Type == HLTokenType.OpBlockToken )
                {
                    if ( !HLParsingTools.ReadOneOrNone(
                                                       tokens,
                                                       i - 1,
                                                       HLTokenType.OpBracketClose,
                                                       out IHLToken bClose
                                                      ) )
                    {
                        continue;
                    }

                    List < IHLToken > argPart = new List < IHLToken > { bClose };
                    IHLToken[] args = HLParsingTools.ReadUntil( tokens, i - 2, -1, HLTokenType.OpBracketOpen );
                    argPart.AddRange( args );

                    IHLToken argOpenBracket = HLParsingTools.ReadOne(
                                                                     tokens,
                                                                     i - 2 - args.Length,
                                                                     HLTokenType.OpBracketOpen
                                                                    );

                    argPart.Add( argOpenBracket );
                    argPart.Reverse();

                    int funcIdx = i - 3 - args.Length;

                    if ( tokens[funcIdx].Type != HLTokenType.OpWord )
                    {
                        continue;
                    }

                    IHLToken funcName = HLParsingTools.ReadOne( tokens, funcIdx, HLTokenType.OpWord );

                    IHLToken typeName = null;

                    if ( funcIdx > 0 &&
                         ( tokens[funcIdx - 1].Type == HLTokenType.OpWord ||
                           tokens[funcIdx - 1].Type == HLTokenType.OpTypeVoid ) )
                    {
                        typeName = HLParsingTools.ReadOneOfAny(
                                                               tokens,
                                                               funcIdx - 1,
                                                               new[] { HLTokenType.OpWord, HLTokenType.OpTypeVoid }
                                                              );

                        int modStart = funcIdx - 1 - 1;

                        IHLToken[] mods = HLParsingTools.ReadNoneOrManyOf(
                                                                          tokens,
                                                                          modStart,
                                                                          -1,
                                                                          settings.MemberModifiers.Values.ToArray()
                                                                         ).
                                                         Reverse().
                                                         ToArray();

                        int start = modStart - mods.Length + 1;
                        int end = i;
                        IHLToken block = tokens[i];
                        tokens.RemoveRange( start, end - start + 1 );

                        tokens.Insert(
                                      start,
                                      new FunctionDefinitionToken(
                                                                  funcName,
                                                                  typeName,
                                                                  ParseArgumentList( args.Reverse().ToList() ),
                                                                  mods,
                                                                  block.GetChildren().ToArray(),
                                                                  start
                                                                 )
                                     );

                        i = start;
                    }
                }
            }
        }

        public void ParseImports( List < IHLToken > tokens )
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                if ( tokens[i].Type == HLTokenType.OpNumSign && tokens.Count > i + 2 )
                {
                    if ( tokens[i + 1].ToString() == "import" && tokens[i + 2].Type == HLTokenType.OpStringLiteral )
                    {
                        ImportedItems.Add( tokens[i + 2].ToString() );
                        tokens.RemoveRange( i, 3 );
                    }
                }
            }
        }

        public void ParseIncludes( List < IHLToken > tokens )
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                if ( tokens[i].Type == HLTokenType.OpNumSign && tokens.Count > i + 2 )
                {
                    if ( tokens[i + 1].ToString() == "include" && tokens[i + 2].Type == HLTokenType.OpStringLiteral )
                    {
                        string c = UriResolver.GetFilePath( Directory, tokens[i + 2].ToString() );
                        IncludedFiles.Add( c ?? tokens[i + 2].ToString() );
                        tokens.RemoveRange( i, 3 );
                    }
                }
            }
        }

        public void ParseOneLineStrings( List < IHLToken > tokens )
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                if ( i < tokens.Count - 1 &&
                     tokens[i].Type == HLTokenType.OpDoubleQuote )
                {
                    int idx = tokens.FindIndex( i + 1, t => t.Type == HLTokenType.OpNewLine );
                    int endQuote = tokens.FindIndex( i + 1, t => t.Type == HLTokenType.OpDoubleQuote );

                    if ( idx == -1 )
                    {
                        idx = tokens.Count - 1;
                    }

                    if ( endQuote == -1 || endQuote > idx )
                    {
                        EventManager < ErrorEvent >.SendEvent(
                                                              new HLTokenReadEvent(
                                                                   HLTokenType.OpDoubleQuote,
                                                                   HLTokenType.OpNewLine
                                                                  )
                                                             );

                        return;
                    }

                    List < IHLToken > content = tokens.GetRange( i + 1, endQuote - i - 1 );

                    string ConcatContent()
                    {
                        return OriginalText.Substring(
                                                      content.First().SourceIndex,
                                                      tokens[i + 1 + content.Count].SourceIndex -
                                                      content.First().SourceIndex
                                                     );
                    }

                    IHLToken newToken = new HLTextToken(
                                                        HLTokenType.OpStringLiteral,
                                                        ConcatContent(),
                                                        tokens[i].SourceIndex
                                                       );

                    tokens.RemoveRange( i, endQuote - i + 1 );
                    tokens.Insert( i, newToken );
                }
            }
        }

        public void ParseVarDefToken( List < IHLToken > tokens, HLParserSettings settings )
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                if ( tokens[i].Type == HLTokenType.OpWord || settings.MemberModifiers.ContainsValue( tokens[i].Type ) )
                {
                    List < IHLToken > line = HLParsingTools.ReadUntilAny(
                                                                         tokens,
                                                                         i,
                                                                         1,
                                                                         new[]
                                                                         {
                                                                             HLTokenType.OpSemicolon,
                                                                             HLTokenType.EOF,
                                                                             HLTokenType.OpBlockBracketOpen,
                                                                             HLTokenType.OpBlockBracketClose
                                                                         }
                                                                        ).
                                                            ToList();

                    IHLToken[] mods =
                        HLParsingTools.ReadNoneOrManyOf( line, 0, 1, settings.MemberModifiers.Values.ToArray() );

                    if ( !HLParsingTools.ReadOneOrNone( line, mods.Length, HLTokenType.OpWord, out IHLToken type ) ||
                         !HLParsingTools.ReadOneOrNone( line, mods.Length + 1, HLTokenType.OpWord, out IHLToken name ) )
                    {
                        i += line.Count;

                        continue;
                    }

                    IHLToken num = null;

                    if ( line.Count == mods.Length + 2 )
                    {
                        tokens.RemoveRange( i, line.Count + 1 );

                        if ( mods.All( x => x.Type != HLTokenType.OpPublicMod ) &&
                             mods.All( x => x.Type != HLTokenType.OpPrivateMod ) )
                        {
                            mods = mods.Append( new HLTextToken( HLTokenType.OpPrivateMod, "private", 0 ) ).ToArray();
                        }

                        tokens.Insert( i, new VariableDefinitionToken( name, type, mods, line.ToArray(), null, null ) );

                        continue;
                    }

                    if ( line[mods.Length + 2].Type == HLTokenType.OpIndexerBracketOpen )
                    {
                        num = HLParsingTools.ReadAny( line, mods.Length + 3 );
                        HLParsingTools.ReadOne( line, mods.Length + 4, HLTokenType.OpIndexerBracketClose );
                        tokens.RemoveRange( i, line.Count + 1 );

                        if ( mods.All( x => x.Type != HLTokenType.OpPublicMod ) &&
                             mods.All( x => x.Type != HLTokenType.OpPrivateMod ) )
                        {
                            mods = mods.Append( new HLTextToken( HLTokenType.OpPrivateMod, "private", 0 ) ).ToArray();
                        }

                        tokens.Insert( i, new VariableDefinitionToken( name, type, mods, line.ToArray(), null, num ) );

                        continue;
                    }

                    if ( line[mods.Length + 2].Type == HLTokenType.OpEquality )
                    {
                        tokens.RemoveRange( i, line.Count + 1 );
                        IHLToken[] init = line.Skip( mods.Length + 3 ).ToArray();

                        if ( mods.All( x => x.Type != HLTokenType.OpPublicMod ) &&
                             mods.All( x => x.Type != HLTokenType.OpPrivateMod ) )
                        {
                            mods = mods.Append( new HLTextToken( HLTokenType.OpPrivateMod, "private", 0 ) ).ToArray();
                        }

                        tokens.Insert(
                                      i,
                                      new VariableDefinitionToken(
                                                                  name,
                                                                  type,
                                                                  mods,
                                                                  line.ToArray(),
                                                                  init,
                                                                  null
                                                                 )
                                     );

                        continue;
                    }

                    i += line.Count;
                }
            }
        }

        internal string GetTempVar( uint initValue )
        {
            VariableData tmp = GetFreeTempVar( initValue );
            usedTempVars.Add( tmp.GetName() );

            return tmp.GetName();
        }

        internal string GetTempVarCopy( string initValue )
        {
            VariableData tmp = GetFreeTempVarCopy( initValue );
            usedTempVars.Add( tmp.GetName() );

            return tmp.GetName();
        }

        internal string GetTempVarDref( string initValue )
        {
            VariableData tmp = GetFreeTempVarDref( initValue );
            usedTempVars.Add( tmp.GetName() );

            return tmp.GetName();
        }

        internal string GetTempVarLoad( string initValue )
        {
            VariableData tmp = GetFreeTempVarLoad( initValue );
            usedTempVars.Add( tmp.GetName() );

            return tmp.GetName();
        }

        internal string GetUniqueName( string prefix = null )
        {
            return ( prefix == null ? "" : prefix + "_" ) + counter++;
        }

        internal string Parse( HLExpression[] block, bool printHead = true, string appendAfterProg = "HLT" )
        {
            foreach ( HLExpression hlExpression in block )
            {
                ExpressionTarget c = Parse( hlExpression, new ExpressionTarget() );
                ReleaseTempVar( c.ResultAddress );
            }

            StringBuilder sb = new StringBuilder();
            StringBuilder funcCode = new StringBuilder();
            GenerateFunctionCode( funcCode, printHead );

            if ( printHead )
            {
                sb.AppendLine( "; ________________ Includes ________________" );
            }

            foreach ( string includedFile in IncludedFiles )
            {
                sb.AppendLine( $":include \"{includedFile}\"" );
            }

            if ( printHead )
            {
                sb.AppendLine( "; ________________ CONST VALUES ________________" );
            }

            foreach ( KeyValuePair < string, string > keyValuePair in ConstValTypes )
            {
                if ( parent == null )
                {
                    sb.AppendLine( $":const {keyValuePair.Key} {keyValuePair.Value} linker:hide" );
                }
                else if ( !parent.ConstValTypes.ContainsKey( keyValuePair.Key ) )
                {
                    parent.ConstValTypes[keyValuePair.Key] = keyValuePair.Value;
                }
            }

            if ( printHead )

            {
                sb.AppendLine( "; ________________ VARIABLE FIELDS ________________" );
            }

            foreach ( KeyValuePair < string, VariableData > keyValuePair in VariableMap )
            {
                if ( parent == null )
                {
                    if ( keyValuePair.Value.InitContent != null )
                    {
                        sb.AppendLine(
                                      $":data {keyValuePair.Value.GetFinalName()} \"{keyValuePair.Value.InitContent}\" linker:hide"
                                     );
                    }
                    else
                    {
                        sb.AppendLine(
                                      $":data {keyValuePair.Value.GetFinalName()} {keyValuePair.Value.Size} {( keyValuePair.Value.IsVisible ? "" : "linker:hide" )}"
                                     );
                    }
                }
                else if ( !parent.VariableMap.ContainsKey( keyValuePair.Key ) )
                {
                    parent.VariableMap[keyValuePair.Key] = keyValuePair.Value;
                }
            }

            if ( printHead )
            {
                sb.AppendLine( "; ________________ MAIN PROGRAM CODE ________________" );
            }

            foreach ( string programCode in ProgramCode )
            {
                sb.AppendLine( programCode );
            }

            if ( !string.IsNullOrEmpty( appendAfterProg ) )
            {
                sb.AppendLine( appendAfterProg );
            }

            sb.Append( funcCode );

            return ParsedText = sb.ToString();
        }

        internal ExpressionTarget Parse( HLExpression expr, ExpressionTarget outputTarget = default )
        {
            Type t = expr.GetType();

            if ( TypeMap.ContainsCompiler( t ) )
            {
                return TypeMap.Get( t ).Parse( this, expr, outputTarget );
            }

            EventManager < ErrorEvent >.SendEvent( new ExpressionCompilerNotFoundEvent( expr ) );

            return new ExpressionTarget();
        }

        internal void ReleaseTempVar( string varName )
        {
            if ( usedTempVars.Contains( varName ) )
            {
                usedTempVars.Remove( varName );
                unusedTempVars.Enqueue( varName );
            }
        }

        #endregion

        #region Private

        private static IHLToken[] ParseArgumentList( List < IHLToken > tokens )
        {
            HLExpressionReader reader = new HLExpressionReader( tokens );

            IHLToken current = reader.GetNext();
            List < VariableDefinitionToken > ret = new List < VariableDefinitionToken >();

            while ( current.Type != HLTokenType.EOF )
            {
                IHLToken typeName = current;
                Eat( HLTokenType.OpWord );
                IHLToken varName = current;
                Eat( HLTokenType.OpWord );

                ret.Add(
                        new VariableDefinitionToken(
                                                    varName,
                                                    typeName,
                                                    new IHLToken[0],
                                                    new[] { typeName, varName },
                                                    null
                                                   )
                       );

                if ( current.Type == HLTokenType.EOF )
                {
                    return ret.ToArray();
                }

                Eat( HLTokenType.OpComma );
            }

            return ret.ToArray();

            void Eat( HLTokenType type )
            {
                if ( current.Type != type )
                {
                    EventManager < ErrorEvent >.SendEvent( new HLTokenReadEvent( type, current.Type ) );
                }

                current = reader.GetNext();
            }
        }

        private void GenerateFunctionCode( StringBuilder sb, bool printHead )
        {
            if ( printHead )
            {
                sb.AppendLine( "; ________________ FUNCTION CODE ________________" );
            }

            foreach ( KeyValuePair < string, FunctionData > keyValuePair in FunctionMap )
            {
                if ( keyValuePair.Value.GetCompiledOutput().Length == 0 )
                {
                    continue;
                }

                if ( parent == null )
                {
                    sb.AppendLine( "." + keyValuePair.Key + ( keyValuePair.Value.Public ? "" : " linker:hide" ) );
                }

                foreach ( string s in keyValuePair.Value.GetCompiledOutput() )
                {
                    if ( parent == null )
                    {
                        sb.AppendLine( s );
                    }
                    else if ( !parent.FunctionMap.ContainsKey( keyValuePair.Key ) )
                    {
                        parent.FunctionMap[keyValuePair.Key] = keyValuePair.Value;
                    }
                }
            }
        }

        private VariableData GetFreeTempVar( uint initValue )
        {
            if ( unusedTempVars.Count != 0 )
            {
                string oldName = unusedTempVars.Dequeue();
                ProgramCode.Add( $"LOAD {oldName} {initValue} ;Temp Var House-keeping" );

                return VariableMap[oldName];
            }

            string name = GetUniqueName( "tmp" );

            if ( initValue != 0 )
            {
                ProgramCode.Add( $"LOAD {name} {initValue} ;Temp Var House-keeping" );
            }

            return VariableMap[name] = new VariableData( name, name, 1, TypeSystem.GetOrAdd( "var" ), false );
        }

        private VariableData GetFreeTempVarCopy( string initValue )
        {
            if ( unusedTempVars.Count != 0 )
            {
                string oldName = unusedTempVars.Dequeue();
                ProgramCode.Add( $"COPY {initValue} {oldName} ;Temp Var House-keeping" );

                return VariableMap[oldName];
            }

            string name = GetUniqueName( "tmp" );

            ProgramCode.Add( $"COPY {initValue} {name} ;Temp Var House-keeping" );

            return VariableMap[name] = new VariableData( name, name, 1, TypeSystem.GetOrAdd( "var" ), false );
        }

        private VariableData GetFreeTempVarDref( string initValue )
        {
            if ( unusedTempVars.Count != 0 )
            {
                string oldName = unusedTempVars.Dequeue();
                ProgramCode.Add( $"DREF {initValue} {oldName} ;Temp Var House-keeping" );

                return VariableMap[oldName];
            }

            string name = GetUniqueName( "tmp" );

            ProgramCode.Add( $"DREF {initValue} {name} ;Temp Var House-keeping" );

            return VariableMap[name] = new VariableData( name, name, 1, TypeSystem.GetOrAdd( "var" ), false );
        }

        private VariableData GetFreeTempVarLoad( string initValue )
        {
            if ( unusedTempVars.Count != 0 )
            {
                string oldName = unusedTempVars.Dequeue();
                ProgramCode.Add( $"LOAD {oldName} {initValue} ;Temp Var House-keeping" );

                return VariableMap[oldName];
            }

            string name = GetUniqueName( "tmp" );

            ProgramCode.Add( $"LOAD {name} {initValue} ;Temp Var House-keeping" );

            return VariableMap[name] = new VariableData( name, name, 1, TypeSystem.GetOrAdd( "var" ), false );
        }

        private string GetPrefix()
        {
            if ( parent == null )
            {
                return scopeID + "_";
            }

            return parent.GetPrefix() + scopeID + "_";
        }

        private void ParseDependencies()
        {
            ExpressionParser exP = new ExpressionParser();

            for ( int i = 0; i < IncludedFiles.Count; i++ )
            {
                string includedFile = IncludedFiles[i];

                if ( parent != null && parent.IncludedFiles.Contains( includedFile ) )
                {
                    continue;
                }

                if ( includedFile.EndsWith( ".vhl" ) )
                {
                    Log( $"Importing File: {includedFile}" );

                    string name = Path.GetFullPath(
                                                   includedFile.StartsWith( Directory )
                                                       ? includedFile.Remove( includedFile.Length - 4, 4 )
                                                       : Directory +
                                                         "/" +
                                                         includedFile.Remove( includedFile.Length - 4, 4 )
                                                  );

                    string newInclude = dataStore.GetStorePath( "HL2VASM", name );
                    string file = Path.GetFullPath( name + ".vhl" );

                    HLCompilation comp = exP.Parse(
                                                   File.ReadAllText( file ),
                                                   Path.GetDirectoryName( file ),
                                                   dataStore
                                                  );

                    File.WriteAllText( newInclude, comp.Parse() );
                    ExternalSymbols.AddRange( comp.FunctionMap.Values.Where( x => x.Public ) );
                    ExternalSymbols.AddRange( comp.ExternalSymbols );
                    includedFile = newInclude;
                }

                OnCompiledIncludedScript?.Invoke(
                                                 Path.GetFullPath( Directory + "/" + IncludedFiles[i] ),
                                                 includedFile
                                                );

                IncludedFiles[i] = includedFile;
            }
        }

        private void ParseReservedKeys( List < IHLToken > tokens, HLParserSettings settings )
        {
            for ( int i = tokens.Count - 1; i >= 0; i-- )
            {
                IHLToken token = tokens[i];

                if ( token.Type == HLTokenType.OpWord && settings.ReservedKeys.ContainsKey( token.ToString() ) )
                {
                    tokens[i] = new HLTextToken(
                                                settings.ReservedKeys[token.ToString()],
                                                token.ToString(),
                                                token.SourceIndex
                                               );
                }
            }
        }

        private void ParseTypeDefinitionBody(
            HLTypeSystem ts,
            HLTypeDefinition tdef,
            List < IHLToken > block )
        {
            HLExpressionParser p = HLExpressionParser.Create( new HLExpressionReader( block ) );
            HLExpression[] exprs = p.Parse();

            foreach ( HLExpression hlToken in exprs )
            {
                if ( hlToken is HLVarDefOperand t )
                {
                    HLTypeDefinition tt = ts.GetOrAdd( t.value.TypeName.ToString() );

                    if ( t.value.Size != null )
                    {
                        tt = new ArrayTypeDefintion(
                                                    tt,
                                                    t.value.Size.ToString().ParseUInt()
                                                   );
                    }

                    HLPropertyDefinition pdef = new HLPropertyDefinition( t.value.Name.ToString(), tt );

                    tdef.AddMember( pdef );
                }
                else
                {
                    EventManager < ErrorEvent >.SendEvent(
                                                          new HLTokenReadEvent(
                                                                               HLTokenType.OpVariableDefinition,
                                                                               hlToken.Type
                                                                              )
                                                         );
                }
            }
        }

        private void ParseTypeDefinitions( HLTypeSystem ts, HLParserSettings settings, List < IHLToken > tokens )
        {
            for ( int i = tokens.Count - 1; i >= 0; i-- )
            {
                if ( tokens[i].Type == HLTokenType.OpBlockToken ||
                     tokens[i].Type == HLTokenType.OpNamespaceDefinition )
                {
                    ParseTypeDefinitions( ts, settings, tokens[i].GetChildren() );
                }

                if ( tokens[i].Type == HLTokenType.OpClass )
                {
                    IHLToken classKey = tokens[i];

                    IHLToken name = HLParsingTools.ReadOne( tokens, i + 1, HLTokenType.OpWord );

                    IHLToken[] mods = HLParsingTools.ReadNoneOrManyOf(
                                                                      tokens,
                                                                      i - 1,
                                                                      -1,
                                                                      settings.ClassModifiers.Values.ToArray()
                                                                     ).
                                                     Reverse().
                                                     ToArray();

                    IHLToken baseClass = null;
                    int offset = 2;

                    if ( HLParsingTools.ReadOneOrNone(
                                                      tokens,
                                                      i + offset,
                                                      HLTokenType.OpColon,
                                                      out IHLToken inhColon
                                                     ) )
                    {
                        baseClass = HLParsingTools.ReadOne( tokens, i + offset + 1, HLTokenType.OpWord );
                        offset += 2;
                    }

                    IHLToken block = HLParsingTools.ReadOne(
                                                            tokens,
                                                            i + offset,
                                                            HLTokenType.OpBlockToken
                                                           );

                    int start = i - mods.Length;
                    int end = i + offset + 1;
                    tokens.RemoveRange( start, end - start );
                    HLTypeDefinition def = ts.CreateEmptyType( name.ToString() );
                    ParseTypeDefinitionBody( ts, def, block.GetChildren() );
                    i = start;
                }
            }
        }

        private void ProcessImports()
        {
            for ( int i = 0; i < ImportedItems.Count; i++ )
            {
                IImporter imp = ImporterSystem.Get( ImportedItems[i] );
                bool error = true;

                if ( imp is IFileImporter fimp )
                {
                    error = false;
                    IncludedFiles.Add( fimp.ProcessImport( ImportedItems[i] ) );
                }

                if ( imp is IDataImporter dimp )
                {
                    error = false;
                    IExternalData[] data = dimp.ProcessImport( ImportedItems[i] );

                    ExternalSymbols.AddRange( data );
                }

                if ( imp == null )
                {
                    EventManager < ErrorEvent >.SendEvent( new ImporterNotFoundEvent( ImportedItems[i] ) );
                }

                if ( error )
                {
                    EventManager < ErrorEvent >.SendEvent( new ImporterTypeInvalidEvent( imp.GetType() ) );
                }
            }
        }

        private void RemoveComments( List < IHLToken > tokens )
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                if ( i < tokens.Count - 1 &&
                     tokens[i].Type == HLTokenType.OpFwdSlash &&
                     tokens[i + 1].Type == HLTokenType.OpFwdSlash )
                {
                    int idx = tokens.FindIndex( i + 2, t => t.Type == HLTokenType.OpNewLine );

                    if ( idx == -1 )
                    {
                        idx = tokens.Count - 1;
                    }

                    tokens.RemoveRange( i, idx - i );
                }
            }
        }

        #endregion

    }

}
