using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using VisCPU.HL.Compiler;
using VisCPU.HL.DataTypes;
using VisCPU.HL.Events;
using VisCPU.HL.Importer;
using VisCPU.HL.Importer.Events;
using VisCPU.HL.Lifetime;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Events;
using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.Parser.Tokens.Combined;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.TypeSystem;
using VisCPU.Instructions.Emit;
using VisCPU.Utility;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.IO.DataStore;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.UriResolvers;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL
{

    public class HlCompilation : VisBase
    {

        public readonly HlTypeSystem TypeSystem = new HlTypeSystem();

        internal readonly Scope <  ConstantValueItem > ConstValTypes =
            new Scope<  ConstantValueItem >();

        internal readonly List < IExternalData > ExternalSymbols = new List < IExternalData >();
        internal readonly Dictionary < string, FunctionData > FunctionMap = new Dictionary < string, FunctionData >();

        internal readonly string OriginalText;

        internal readonly EmitterResult < string > EmitterResult = new EmitterResult < string >( new TextEmitter() );

        internal HlCompilerCollection TypeMap;

        private static uint s_Counter;

        private readonly HlCompilerSettings m_Settings = SettingsManager.GetSettings < HlCompilerSettings >();
        private readonly string m_Directory;

        private readonly List < string > m_IncludedFiles = new List < string >();
        private readonly List < string > m_ImportedItems = new List < string >();
        private readonly HlCompilation m_Parent;

        private readonly Queue < string > m_UnusedTempVars = new Queue < string >();
        private readonly List < string > m_UsedTempVars = new List < string >();
        private readonly Scope < VariableData > m_VariableMap = new Scope < VariableData >( );
        private string m_ParsedText;

        private readonly BuildDataStore m_DataStore;

        public event Action < string, string > OnCompiledIncludedScript;

        protected override LoggerSystems SubSystem => LoggerSystems.HlCompiler;

        #region Public

        public HlCompilation( string originalText, string directory ) : this(
                                                                             originalText,
                                                                             directory,
                                                                             new BuildDataStore(
                                                                                  directory,
                                                                                  new HlBuildDataStore()
                                                                                 )
                                                                            )
        {
        }

        public HlCompilation( string originalText, string directory, BuildDataStore dataStore )
        {
            m_DataStore = dataStore;
            OriginalText = originalText;
            m_Directory = directory;
            TypeMap = new HlCompilerCollection( TypeSystem );
        }

        public HlCompilation( HlCompilation parent, string id )
        {
            m_DataStore = parent.m_DataStore;
            OriginalText = parent.OriginalText;
            m_Directory = parent.m_Directory;
            m_VariableMap = new Scope < VariableData >( parent.m_VariableMap, id );
            ConstValTypes = new Scope< ConstantValueItem >( parent.ConstValTypes, id );
            FunctionMap = new Dictionary < string, FunctionData >( parent.FunctionMap );
            ExternalSymbols = new List < IExternalData >( parent.ExternalSymbols );
            m_IncludedFiles = new List < string >( parent.m_IncludedFiles );
            TypeSystem = parent.TypeSystem;
            m_Parent = parent;
            TypeMap = new HlCompilerCollection( TypeSystem );
        }

        public static void ResetCounter()
        {
            s_Counter = 0;
        }

        public string GetFinalName( string name )
        {
            if ( m_VariableMap.Contains( name ) )
            {
                return m_VariableMap.Get( name ).GetFinalName();
            }
            return m_VariableMap.GetFinalName( name );
        }

        internal static string GetUniqueName( string prefix = null )
        {
            return ( prefix == null ? "" : prefix + "_" ) + s_Counter++;
        }

        public bool ContainsLocalVariable( string var )
        {
            return m_VariableMap.ContainsLocal(  var  );
        }

        public bool ContainsVariable( string var )
        {
            return m_VariableMap.Contains(  var  );
        }

        public void CreateVariable( string name, uint size, HlTypeDefinition tdef, bool isVisible )
        {
            m_VariableMap.Set(
                              name ,
                              new VariableData(
                                               name,
                                               m_VariableMap.GetFinalName( name ),
                                               size,
                                               tdef,
                                               isVisible
                                              )
                             );
        }

        public void CreateVariable( string name, string content, HlTypeDefinition tdef, bool isVisible )
        {
            m_VariableMap.Set(
                               name ,
                              new VariableData( name, m_VariableMap.GetFinalName(name) , content, tdef, isVisible )
                             );
        }
        
        public VariableData GetVariable( string name )
        {
            if ( m_VariableMap.Contains(  name  ) )
            {
                return m_VariableMap.Get( name );
            }
            EventManager<ErrorEvent>.SendEvent(new HlVariableNotFoundEvent(name, false));

            return new VariableData();
        }

        public string Parse( bool printHead = true, string appendAfterProg = "HLT" )
        {
            if ( m_ParsedText != null )
            {
                return m_ParsedText;
            }

            HlParserSettings hlpS = new HlParserSettings();
            HlParserBaseReader br = new HlParserBaseReader( hlpS, OriginalText );

            List < IHlToken > tokens = br.ReadToEnd();
            ParseOneLineStrings( tokens );
            ParseCharTokens( tokens );
            RemoveComments( tokens );
            ParseImports( tokens );
            ParseIncludes( tokens );
            ParseReservedKeys( tokens, hlpS );
            tokens = tokens.Where( x => x.Type != HlTokenType.OpNewLine ).ToList();
            ParseVarDefToken( tokens, hlpS );
            ParseBlocks( tokens );

            ParseFunctionToken( tokens, hlpS );
            ParseTypeDefinitions( TypeSystem, hlpS, tokens );

            //foreach ( HlTypeDefinition type in TypeSystem )
            //{
            //    foreach ( HlMemberDefinition member in type )
            //    {
            //        if ( member is HlFunctionDefinition fdef )
            //        {
            //            string name = $"{type}_{member.Name}";
            //            FunctionMap[name] = new FunctionData()
            //        }
            //    }
            //}

            HlExpressionParser p = HlExpressionParser.Create( new HlExpressionReader( tokens ) );
            ProcessImports();
            ParseDependencies();

            return Parse( p.Parse(), printHead, appendAfterProg );
        }

        public void ParseBlocks( List < IHlToken > tokens )
        {
            for ( int i = tokens.Count - 1; i >= 0; i-- )
            {
                if ( tokens[i].Type == HlTokenType.OpBlockBracketClose )
                {
                    int current = 1;
                    int start = i - 1;

                    for ( ; start >= 0; start-- )
                    {
                        if ( tokens[start].Type == HlTokenType.OpBlockBracketClose )
                        {
                            current++;
                        }
                        else if ( tokens[start].Type == HlTokenType.OpBlockBracketOpen )
                        {
                            current--;

                            if ( current == 0 )
                            {
                                break;
                            }
                        }
                    }

                    List < IHlToken > content = tokens.GetRange( start + 1, i - start - 1 ).ToList();
                    tokens.RemoveRange( start, i - start + 1 );
                    ParseBlocks( content );
                    tokens.Insert( start, new BlockToken( content.ToArray(), start ) );
                    i = start;
                }
            }
        }

        public void ParseCharTokens( List < IHlToken > tokens )
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                if ( i < tokens.Count - 1 &&
                     tokens[i].Type == HlTokenType.OpSingleQuote )
                {
                    int idx = tokens.FindIndex( i + 1, t => t.Type == HlTokenType.OpNewLine );
                    int endQuote = tokens.FindIndex( i + 1, t => t.Type == HlTokenType.OpSingleQuote );

                    if ( idx == -1 )
                    {
                        idx = tokens.Count - 1;
                    }

                    if ( endQuote == -1 || endQuote > idx )
                    {
                        EventManager < ErrorEvent >.SendEvent(
                                                              new HlTokenReadEvent(
                                                                   HlTokenType.OpSingleQuote,
                                                                   HlTokenType.OpNewLine
                                                                  )
                                                             );

                        return;
                    }

                    string ConcatContent()
                    {
                        List < IHlToken > content = tokens.GetRange( i + 1, endQuote - i - 1 );

                        return OriginalText.Substring(
                                                      content.First().SourceIndex,
                                                      tokens[i + 1 + content.Count].SourceIndex -
                                                      content.First().SourceIndex
                                                     );
                    }

                    IHlToken newToken = new HlTextToken(
                                                        HlTokenType.OpCharLiteral,
                                                        ConcatContent(),
                                                        tokens[i].SourceIndex
                                                       );

                    tokens.RemoveRange( i, endQuote - i + 1 );
                    tokens.Insert( i, newToken );
                }
            }
        }

        public void ParseFunctionToken( List < IHlToken > tokens, HlParserSettings settings, HlTypeDefinition tdef=null)
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                if ( tokens[i].Type == HlTokenType.OpBlockToken )
                {
                    if ( !HlParsingTools.ReadOneOrNone(
                                                       tokens,
                                                       i - 1,
                                                       HlTokenType.OpBracketClose,
                                                       out IHlToken bClose
                                                      ) )
                    {
                        continue;
                    }

                    List < IHlToken > argPart = new List < IHlToken > { bClose };
                    IHlToken[] args = HlParsingTools.ReadUntil( tokens, i - 2, -1, HlTokenType.OpBracketOpen );
                    argPart.AddRange( args );

                    IHlToken argOpenBracket = HlParsingTools.ReadOne(
                                                                     tokens,
                                                                     i - 2 - args.Length,
                                                                     HlTokenType.OpBracketOpen
                                                                    );

                    argPart.Add( argOpenBracket );
                    argPart.Reverse();

                    int funcIdx = i - 3 - args.Length;

                    if ( tokens[funcIdx].Type != HlTokenType.OpWord )
                    {
                        continue;
                    }

                    IHlToken funcName = HlParsingTools.ReadOne( tokens, funcIdx, HlTokenType.OpWord );

                    IHlToken typeName = null;

                    if ( funcIdx > 0 &&
                         ( tokens[funcIdx - 1].Type == HlTokenType.OpWord ||
                           tokens[funcIdx - 1].Type == HlTokenType.OpTypeVoid ) )
                    {
                        typeName = HlParsingTools.ReadOneOfAny(
                                                               tokens,
                                                               funcIdx - 1,
                                                               new[] { HlTokenType.OpWord, HlTokenType.OpTypeVoid }
                                                              );

                        int modStart = funcIdx - 1 - 1;

                        IHlToken[] mods = HlParsingTools.ReadNoneOrManyOf(
                                                                          tokens,
                                                                          modStart,
                                                                          -1,
                                                                          settings.MemberModifiers.Values.ToArray()
                                                                         ).
                                                         Reverse().
                                                         ToArray();

                        int start = modStart - mods.Length + 1;
                        int end = i;
                        IHlToken block = tokens[i];
                        tokens.RemoveRange( start, end - start + 1 );

                        tokens.Insert(
                                      start,
                                      new FunctionDefinitionToken(
                                                                  funcName,
                                                                  typeName,
                                                                  ParseArgumentList( args.Reverse().ToList() ),
                                                                  mods,
                                                                  block.GetChildren().ToArray(),
                                                                  start,
                                                                  tdef
                                                                 )
                                     );

                        i = start;
                    }
                }
            }
        }

        public void ParseImports( List < IHlToken > tokens )
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                if ( tokens[i].Type == HlTokenType.OpNumSign && tokens.Count > i + 2 )
                {
                    if ( tokens[i + 1].ToString() == "import" && tokens[i + 2].Type == HlTokenType.OpStringLiteral )
                    {
                        m_ImportedItems.Add( tokens[i + 2].ToString() );
                        tokens.RemoveRange( i, 3 );
                    }
                }
            }
        }

        public void ParseIncludes( List < IHlToken > tokens )
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                if ( tokens[i].Type == HlTokenType.OpNumSign && tokens.Count > i + 2 )
                {
                    if ( tokens[i + 1].ToString() == "include" && tokens[i + 2].Type == HlTokenType.OpStringLiteral )
                    {
                        string c = UriResolver.GetFilePath( m_Directory, tokens[i + 2].ToString() );
                        m_IncludedFiles.Add( c ?? tokens[i + 2].ToString() );
                        tokens.RemoveRange( i, 3 );
                    }
                }
            }
        }

        public void ParseOneLineStrings( List < IHlToken > tokens )
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                if ( i < tokens.Count - 1 &&
                     tokens[i].Type == HlTokenType.OpDoubleQuote )
                {
                    int idx = tokens.FindIndex( i + 1, t => t.Type == HlTokenType.OpNewLine );
                    int endQuote = tokens.FindIndex( i + 1, t => t.Type == HlTokenType.OpDoubleQuote );

                    if ( idx == -1 )
                    {
                        idx = tokens.Count - 1;
                    }

                    if ( endQuote == -1 || endQuote > idx )
                    {
                        EventManager < ErrorEvent >.SendEvent(
                                                              new HlTokenReadEvent(
                                                                   HlTokenType.OpDoubleQuote,
                                                                   HlTokenType.OpNewLine
                                                                  )
                                                             );

                        return;
                    }

                    string ConcatContent()
                    {
                        List < IHlToken > content = tokens.GetRange( i + 1, endQuote - i - 1 );

                        return OriginalText.Substring(
                                                      content.First().SourceIndex,
                                                      tokens[i + 1 + content.Count].SourceIndex -
                                                      content.First().SourceIndex
                                                     );
                    }

                    IHlToken newToken = new HlTextToken(
                                                        HlTokenType.OpStringLiteral,
                                                        ConcatContent(),
                                                        tokens[i].SourceIndex
                                                       );

                    tokens.RemoveRange( i, endQuote - i + 1 );
                    tokens.Insert( i, newToken );
                }
            }
        }

        public void ParseVarDefToken( List < IHlToken > tokens, HlParserSettings settings )
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                if ( tokens[i].Type == HlTokenType.OpWord || settings.MemberModifiers.ContainsValue( tokens[i].Type ) )
                {
                    List < IHlToken > line = HlParsingTools.ReadUntilAny(
                                                                         tokens,
                                                                         i,
                                                                         1,
                                                                         new[]
                                                                         {
                                                                             HlTokenType.OpSemicolon,
                                                                             HlTokenType.Eof,
                                                                             HlTokenType.OpBlockBracketOpen,
                                                                             HlTokenType.OpBlockBracketClose
                                                                         }
                                                                        ).
                                                            ToList();

                    IHlToken[] mods =
                        HlParsingTools.ReadNoneOrManyOf( line, 0, 1, settings.MemberModifiers.Values.ToArray() );

                    if ( !HlParsingTools.ReadOneOrNone( line, mods.Length, HlTokenType.OpWord, out IHlToken type ) ||
                         !HlParsingTools.ReadOneOrNone( line, mods.Length + 1, HlTokenType.OpWord, out IHlToken name ) )
                    {
                        i += line.Count;

                        continue;
                    }

                    IHlToken num = null;

                    if ( line.Count == mods.Length + 2 )
                    {
                        tokens.RemoveRange( i, line.Count + 1 );

                        if ( mods.All( x => x.Type != HlTokenType.OpPublicMod ) &&
                             mods.All( x => x.Type != HlTokenType.OpPrivateMod ) )
                        {
                            mods = mods.Append( new HlTextToken( HlTokenType.OpPrivateMod, "private", 0 ) ).ToArray();
                        }

                        tokens.Insert( i, new VariableDefinitionToken( name, type, mods, line.ToArray(), null, null ) );

                        continue;
                    }

                    if ( line[mods.Length + 2].Type == HlTokenType.OpIndexerBracketOpen )
                    {
                        num = HlParsingTools.ReadAny( line, mods.Length + 3 );
                        HlParsingTools.ReadOne( line, mods.Length + 4, HlTokenType.OpIndexerBracketClose );
                        tokens.RemoveRange( i, line.Count + 1 );

                        if ( mods.All( x => x.Type != HlTokenType.OpPublicMod ) &&
                             mods.All( x => x.Type != HlTokenType.OpPrivateMod ) )
                        {
                            mods = mods.Append( new HlTextToken( HlTokenType.OpPrivateMod, "private", 0 ) ).ToArray();
                        }

                        tokens.Insert( i, new VariableDefinitionToken( name, type, mods, line.ToArray(), null, num ) );

                        continue;
                    }

                    if ( line[mods.Length + 2].Type == HlTokenType.OpEquality )
                    {
                        tokens.RemoveRange( i, line.Count + 1 );
                        IHlToken[] init = line.Skip( mods.Length + 3 ).ToArray();

                        if ( mods.All( x => x.Type != HlTokenType.OpPublicMod ) &&
                             mods.All( x => x.Type != HlTokenType.OpPrivateMod ) )
                        {
                            mods = mods.Append( new HlTextToken( HlTokenType.OpPrivateMod, "private", 0 ) ).ToArray();
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
            m_UsedTempVars.Add( tmp.GetName() );

            return tmp.GetName();
        }

        internal string GetTempVarCopy( string initValue )
        {
            VariableData tmp = GetFreeTempVarCopy( initValue );
            m_UsedTempVars.Add( tmp.GetName() );

            return tmp.GetName();
        }

        internal string GetTempVarDref( string initValue )
        {
            VariableData tmp = GetFreeTempVarDref( initValue );
            m_UsedTempVars.Add( tmp.GetName() );

            return tmp.GetName();
        }

        internal string GetTempVarLoad( string initValue )
        {
            VariableData tmp = GetFreeTempVarLoad( initValue );
            m_UsedTempVars.Add( tmp.GetName() );

            return tmp.GetName();
        }

        internal string GetTempVarPop()
        {
            VariableData tmp = GetFreeTempVarPop();
            m_UsedTempVars.Add( tmp.GetName() );

            return tmp.GetName();
        }

        internal string Parse( HlExpression[] block, bool printHead = true, string appendAfterProg = "HLT" )
        {
            foreach ( HlExpression hlExpression in block )
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

            foreach ( string includedFile in m_IncludedFiles )
            {
                sb.AppendLine( $":include \"{includedFile}\"" );
            }

            if ( printHead )
            {
                sb.AppendLine( "; ________________ CONST VALUES ________________" );
            }

            foreach ( KeyValuePair < string, ConstantValueItem > keyValuePair in ConstValTypes )
            {
                    sb.AppendLine(
                                  $":const {keyValuePair.Key} {keyValuePair.Value.Value} {( keyValuePair.Value.IsPublic ? "" : "linker:hide" )}"
                                 );
                
            }

            EmitVariables( printHead).ForEach(x=>sb.AppendLine(x));
            

            if ( printHead )
            {
                sb.AppendLine( "; ________________ MAIN PROGRAM CODE ________________" );
            }

            string[] code = EmitterResult.Get();

            foreach ( string programCode in code )
            {
                sb.AppendLine( programCode );
            }

            if ( !string.IsNullOrEmpty( appendAfterProg ) )
            {
                sb.AppendLine( appendAfterProg );
            }

            sb.Append( funcCode );

            return m_ParsedText = sb.ToString();
        }

        public List <string> EmitVariables( bool printHead)
        {
            List < string > sb = new List < string >();
            if (printHead)
            {
                sb.Add("; ________________ VARIABLE FIELDS ________________");
            }

            foreach (KeyValuePair<string, VariableData> keyValuePair in m_VariableMap)
            {
                if (keyValuePair.Value.InitContent != null)
                {
                    sb.Add(
                           $":data {keyValuePair.Value.GetFinalName()} \"{keyValuePair.Value.InitContent}\" linker:hide"
                          );
                }
                else
                {
                    sb.Add(
                           $":data {keyValuePair.Value.GetFinalName()} {keyValuePair.Value.Size} {(keyValuePair.Value.IsVisible ? "" : "linker:hide")}"
                          );
                }
            }

            return sb;
        }

        internal ExpressionTarget Parse( HlExpression expr, ExpressionTarget outputTarget = default )
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
            if ( m_Settings.OptimizeTempVarUsage && m_UsedTempVars.Contains( varName ) )
            {
                m_UsedTempVars.Remove( varName );
                m_UnusedTempVars.Enqueue( varName );
            }
        }

        #endregion

        #region Private

        private static IHlToken[] ParseArgumentList( List < IHlToken > tokens )
        {
            HlExpressionReader reader = new HlExpressionReader( tokens );

            IHlToken current = reader.GetNext();
            List < VariableDefinitionToken > ret = new List < VariableDefinitionToken >();

            while ( current.Type != HlTokenType.Eof )
            {
                IHlToken typeName = current;
                Eat( HlTokenType.OpWord );
                IHlToken varName = current;
                Eat( HlTokenType.OpWord );

                ret.Add(
                        new VariableDefinitionToken(
                                                    varName,
                                                    typeName,
                                                    new IHlToken[0],
                                                    new[] { typeName, varName },
                                                    null
                                                   )
                       );

                if ( current.Type == HlTokenType.Eof )
                {
                    return ret.ToArray();
                }

                Eat( HlTokenType.OpComma );
            }

            return ret.Cast < IHlToken >().ToArray();

            void Eat( HlTokenType type )
            {
                if ( current.Type != type )
                {
                    EventManager < ErrorEvent >.SendEvent( new HlTokenReadEvent( type, current.Type ) );
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

                if ( m_Parent == null )
                {
                    sb.AppendLine( "." + keyValuePair.Key + ( keyValuePair.Value.Public ? "" : " linker:hide" ) );
                }

                foreach ( string s in keyValuePair.Value.GetCompiledOutput() )
                {
                    if ( m_Parent == null )
                    {
                        sb.AppendLine( s );
                    }
                    else if ( !m_Parent.FunctionMap.ContainsKey( keyValuePair.Key ) )
                    {
                        m_Parent.FunctionMap[keyValuePair.Key] = keyValuePair.Value;
                    }
                }
            }
        }

        private VariableData GetFreeTempVar( uint initValue )
        {
            if ( m_UnusedTempVars.Count != 0 )
            {
                string oldName = m_UnusedTempVars.Dequeue();

                EmitterResult.Emit( "LOAD", oldName, initValue.ToString() );

                return m_VariableMap.Get(oldName);
            }

            string name = GetUniqueName( "tmp" );

            if ( initValue != 0 )
            {
                EmitterResult.Emit( $"LOAD", name, initValue.ToString() );
            }



            return m_VariableMap.Set(
                                     name,
                                     new VariableData(
                                                      name,
                                                      name,
                                                      1,
                                                      TypeSystem.GetType( HLBaseTypeNames.s_UintTypeName ),
                                                      false
                                                     )
                                    );
        }

        private VariableData GetFreeTempVarCopy( string initValue )
        {
            if ( m_UnusedTempVars.Count != 0 )
            {
                string oldName = m_UnusedTempVars.Dequeue();
                EmitterResult.Emit( $"COPY", initValue, oldName );

                return m_VariableMap.Get(oldName);
            }

            string name = GetUniqueName( "tmp" );

            EmitterResult.Emit( $"COPY", initValue, name );



            return m_VariableMap.Set(
                                     name,
                                     new VariableData(
                                                      name,
                                                      name,
                                                      1,
                                                      TypeSystem.GetType( HLBaseTypeNames.s_UintTypeName ),
                                                      false
                                                     )
                                    );
        }

        private VariableData GetFreeTempVarDref( string initValue )
        {
            if ( m_UnusedTempVars.Count != 0 )
            {
                string oldName = m_UnusedTempVars.Dequeue();
                EmitterResult.Emit( $"DREF", initValue, oldName );

                return m_VariableMap.Get(oldName);
            }

            string name = GetUniqueName( "tmp" );

            EmitterResult.Emit( $"DREF", initValue, name );



            return m_VariableMap.Set(
                                     name,
                                     new VariableData(
                                                      name,
                                                      name,
                                                      1,
                                                      TypeSystem.GetType( HLBaseTypeNames.s_UintTypeName ),
                                                      false
                                                     )
                                    );
        }

        private VariableData GetFreeTempVarLoad( string initValue )
        {
            if ( m_UnusedTempVars.Count != 0 )
            {
                string oldName = m_UnusedTempVars.Dequeue();
                EmitterResult.Emit( $"LOAD", oldName, initValue );

                return m_VariableMap.Get(oldName);
            }

            string name = GetUniqueName( "tmp" );

            EmitterResult.Emit( $"LOAD", name, initValue );


           return m_VariableMap.Set(
                              name,
                              new VariableData(
                                               name,
                                               name,
                                               1,
                                               TypeSystem.GetType( HLBaseTypeNames.s_UintTypeName ),
                                               false
                                              )
                             );
        }

        private VariableData GetFreeTempVarPop()
        {
            if ( m_UnusedTempVars.Count != 0 )
            {
                string oldName = m_UnusedTempVars.Dequeue();
                EmitterResult.Emit( "POP", oldName );

                return m_VariableMap.Get(oldName);
            }

            string name = GetUniqueName( "tmp" );

            EmitterResult.Emit( "POP", name );

         return   m_VariableMap.Set(
                              name,
                              new VariableData(
                                               name,
                                               name,
                                               1,
                                               TypeSystem.GetType( HLBaseTypeNames.s_UintTypeName ),
                                               false
                                              )
                             );
        }


        private void ParseDependencies()
        {
            ExpressionParser exP = new ExpressionParser();

            for ( int i = 0; i < m_IncludedFiles.Count; i++ )
            {
                string includedFile = m_IncludedFiles[i];

                if ( m_Parent != null && m_Parent.m_IncludedFiles.Contains( includedFile ) )
                {
                    continue;
                }

                if ( includedFile.EndsWith( ".vhl" ) )
                {
                    Log( $"Importing File: {includedFile}" );

                    string name = Path.GetFullPath(
                                                   includedFile.StartsWith( m_Directory )
                                                       ? includedFile.Remove( includedFile.Length - 4, 4 )
                                                       : m_Directory +
                                                         "/" +
                                                         includedFile.Remove( includedFile.Length - 4, 4 )
                                                  );

                    UriKind kind = includedFile.StartsWith( "/" ) || includedFile.StartsWith( "\\" )
                                       ? UriKind.Absolute
                                       : UriKind.RelativeOrAbsolute;

                    Log( "Detected Uri Kind: {0}", kind );
                    Uri import = null;

                    if ( kind == UriKind.Absolute )
                    {
                        import = new Uri( "file://" + includedFile, kind );
                    }
                    else
                    {
                        import = new Uri( includedFile, kind );
                    }

                    Uri dir = new Uri( "file://" + Directory.GetCurrentDirectory() + "/", UriKind.Absolute );

                    if ( import.IsAbsoluteUri )
                    {
                        Log( $"Relative Base Path: {dir.OriginalString}" );
                        name = dir.MakeRelativeUri( import ).OriginalString;
                        name = name.Remove( name.Length - 4, 4 );
                        Log( $"Fixed Path to File: {includedFile} => {name}" );
                    }

                    string newInclude = m_DataStore.GetStorePath( "HL2VASM", name );
                    string file = Path.GetFullPath( name + ".vhl" );

                    HlCompilation comp = exP.Parse(
                                                   File.ReadAllText( file ),
                                                   Path.GetDirectoryName( file ),
                                                   m_DataStore
                                                  );

                    File.WriteAllText( newInclude, comp.Parse() );

                    TypeSystem.Import( comp.TypeSystem );

                    ExternalSymbols.AddRange(
                                             comp.ConstValTypes.Where( x => x.Value.IsPublic ).
                                                  Select(
                                                         x => new ConstantData(
                                                                               x.Key,
                                                                               x.Key,
                                                                               x.Value.Value,
                                                                               TypeSystem.GetType(
                                                                                    HLBaseTypeNames.s_UintTypeName
                                                                                   ),
                                                                               true
                                                                              )
                                                        ).
                                                  Cast < IExternalData >()
                                            );

                    ExternalSymbols.AddRange( comp.FunctionMap.Values.Where( x => x.Public ) );
                    ExternalSymbols.AddRange( comp.ExternalSymbols );

                    includedFile = newInclude;
                }

                OnCompiledIncludedScript?.Invoke(
                                                 Path.GetFullPath( m_Directory + "/" + m_IncludedFiles[i] ),
                                                 includedFile
                                                );

                m_IncludedFiles[i] = includedFile;
            }
        }

        private void ParseReservedKeys( List < IHlToken > tokens, HlParserSettings settings )
        {
            for ( int i = tokens.Count - 1; i >= 0; i-- )
            {
                IHlToken token = tokens[i];

                if ( token.Type == HlTokenType.OpWord && settings.ReservedKeys.ContainsKey( token.ToString() ) )
                {
                    tokens[i] = new HlTextToken(
                                                settings.ReservedKeys[token.ToString()],
                                                token.ToString(),
                                                token.SourceIndex
                                               );
                }
            }
        }

        private void ParseTypeDefinitionBody(
            HlTypeSystem ts,
            HlTypeDefinition tdef,
            List < IHlToken > block, HlParserSettings settings )
        {
            ParseFunctionToken( block, settings, tdef );
            HlExpressionParser p = HlExpressionParser.Create( new HlExpressionReader( block ) );
            HlExpression[] exprs = p.Parse();

            foreach ( HlExpression hlToken in exprs )
            {
                if ( hlToken is HlVarDefOperand t )
                {
                    HlTypeDefinition tt = ts.GetType( t.VariableDefinition.TypeName.ToString() );

                    if ( t.VariableDefinition.Size != null )
                    {
                        tt = new ArrayTypeDefintion(
                                                    tt,
                                                    t.VariableDefinition.Size.ToString().ParseUInt()
                                                   );
                    }

                    HlPropertyDefinition pdef = new HlPropertyDefinition(
                                                                         t.VariableDefinition.Name.ToString(),
                                                                         tt,
                                                                         t.VariableDefinition.Modifiers
                                                                        );

                    tdef.AddMember( pdef );
                }
                else if ( hlToken is HlFuncDefOperand fdef )
                {
                    if(tdef.HasMember(fdef.FunctionDefinition.FunctionName.ToString()))
                        continue;

                    tdef.AddMember(
                                   new HlFunctionDefinition(
                                                            fdef.FunctionDefinition.FunctionName.ToString(),
                                                            ts.GetType(
                                                                       fdef.FunctionDefinition.FunctionReturnType.
                                                                            ToString()
                                                                      ),
                                                            fdef.FunctionDefinition.Arguments.
                                                                 Select( x => ts.GetType(x.GetChildren().First().ToString() ) ).
                                                                 ToArray(),
                                                            fdef.FunctionDefinition.Mods
                                                           )
                                  );

                    string funcName = $"FUN_{tdef.Name}_{fdef.FunctionDefinition.FunctionName}";
                    HlCompilation fComp = new HlCompilation(this, funcName);

                    FunctionMap[funcName] = new FunctionData(
                                                                 funcName,
                                                                 fdef.FunctionDefinition.Mods.Any(x=>x.Type==HlTokenType.OpPublicMod),
                                                                 () =>
                                                                 {
                                                                     Log($"Importing Function: {funcName}");

                                                                     foreach (IHlToken valueArgument in fdef.FunctionDefinition.Arguments)
                                                                     {
                                                                         VariableDefinitionToken vdef = valueArgument as VariableDefinitionToken;
                                                                         string key = vdef.Name.ToString();

                                                                         fComp.CreateVariable(
                                                                              key,
                                                                              1,
                                                                              TypeSystem.GetType(vdef.TypeName.ToString()),
                                                                              false
                                                                             );
                                                                     }

                                                                     List<string> parsedVal =
                                                                         fComp.Parse(fdef.Block, false, null).Replace("\r", "").Split('\n').ToList();

                                                                     foreach (IHlToken valueArgument in fdef.FunctionDefinition.Arguments)
                                                                     {
                                                                         parsedVal.Insert(
                                                                              0,
                                                                              $"POP {fComp.GetFinalName((valueArgument as VariableDefinitionToken).Name.ToString())}"
                                                                             );
                                                                     }

                                                                     parsedVal.Add("PUSH 0 ; Push anything. Will not be used anyway.");
                                                                     parsedVal.Add("RET ; Compiler Safeguard.");

                                                                     return parsedVal.ToArray();
                                                                 },
                                                                 fdef.FunctionDefinition.Arguments.Length,
                                                                 fdef.FunctionDefinition.FunctionReturnType.ToString()
                                                                );
                }
                else 
                {
                    EventManager < ErrorEvent >.SendEvent(
                                                          new HlTokenReadEvent(
                                                                               HlTokenType.OpVariableDefinition,
                                                                               hlToken.Type
                                                                              )
                                                         );
                }
            }
        }

        private void ParseTypeDefinitions( HlTypeSystem ts, HlParserSettings settings, List < IHlToken > tokens )
        {
            for ( int i = tokens.Count - 1; i >= 0; i-- )
            {
                if ( tokens[i].Type == HlTokenType.OpBlockToken ||
                     tokens[i].Type == HlTokenType.OpNamespaceDefinition )
                {
                    ParseTypeDefinitions( ts, settings, tokens[i].GetChildren() );
                }

                if ( tokens[i].Type == HlTokenType.OpClass )
                {
                    IHlToken name = HlParsingTools.ReadOne( tokens, i + 1, HlTokenType.OpWord );

                    IHlToken[] mods = HlParsingTools.ReadNoneOrManyOf(
                                                                      tokens,
                                                                      i - 1,
                                                                      -1,
                                                                      settings.ClassModifiers.Values.ToArray()
                                                                     ).
                                                     Reverse().
                                                     ToArray();

                    IHlToken baseClass = null;
                    int offset = 2;

                    if ( HlParsingTools.ReadOneOrNone(
                                                      tokens,
                                                      i + offset,
                                                      HlTokenType.OpColon,
                                                      out IHlToken inhColon
                                                     ) )
                    {
                        baseClass = HlParsingTools.ReadOne( tokens, i + offset + 1, HlTokenType.OpWord );
                        Log( "Found base class: {0}", baseClass );
                        offset += 2;
                    }

                    IHlToken block = HlParsingTools.ReadOne(
                                                            tokens,
                                                            i + offset,
                                                            HlTokenType.OpBlockToken
                                                           );

                    int start = i - mods.Length;
                    int end = i + offset + 1;
                    tokens.RemoveRange( start, end - start );

                    HlTypeDefinition def = ts.CreateEmptyType(
                                                              name.ToString(),
                                                              mods.Any( x => x.Type == HlTokenType.OpPublicMod )
                                                             );
                    ParseTypeDefinitionBody( ts, def, block.GetChildren(), settings );
                    i = start;
                }
            }
        }

        private void ProcessImports()
        {
            for ( int i = 0; i < m_ImportedItems.Count; i++ )
            {
                IImporter imp = ImporterSystem.Get( m_ImportedItems[i] );
                bool error = true;

                if ( imp is IFileImporter fimp )
                {
                    error = false;
                    m_IncludedFiles.Add( fimp.ProcessImport( m_ImportedItems[i] ) );
                }

                if ( imp is IDataImporter dimp )
                {
                    error = false;
                    IExternalData[] data = dimp.ProcessImport(this,  m_ImportedItems[i] );

                    ExternalSymbols.AddRange( data );
                }

                if ( imp == null )
                {
                    EventManager < ErrorEvent >.SendEvent( new ImporterNotFoundEvent( m_ImportedItems[i] ) );
                }

                if ( error )
                {
                    EventManager < ErrorEvent >.SendEvent( new ImporterTypeInvalidEvent( imp.GetType() ) );
                }
            }
        }

        private void RemoveComments( List < IHlToken > tokens )
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                if ( i < tokens.Count - 1 &&
                     tokens[i].Type == HlTokenType.OpFwdSlash &&
                     tokens[i + 1].Type == HlTokenType.OpFwdSlash )
                {
                    int idx = tokens.FindIndex( i + 2, t => t.Type == HlTokenType.OpNewLine );

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
