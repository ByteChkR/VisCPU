using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VisCPU.HL.Compiler;
using VisCPU.HL.Compiler.Special;
using VisCPU.HL.DataTypes;
using VisCPU.HL.Events;
using VisCPU.HL.Importer;
using VisCPU.HL.Importer.Events;
using VisCPU.HL.Lifetime;
using VisCPU.HL.Namespaces;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Events;
using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.Parser.Tokens.Combined;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.TextLoader;
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
        public readonly HlTypeSystem TypeSystem;
        public readonly HlNamespace Root = new RootNamespace();

        internal readonly Scope < ConstantValueItem > ConstValTypes =
            new Scope < ConstantValueItem >();

        internal readonly Scope < FunctionData > FunctionMap = new Scope < FunctionData >();

        internal string OriginalText;

        internal readonly EmitterResult < string > EmitterResult = new EmitterResult < string >( new TextEmitter() );

        internal HlCompilerCollection TypeMap;

        private static uint s_Counter;

        private readonly List < string > m_LocalCompilationFlags = new List < string >();

        private List < IExternalData > externalSymbols = new List < IExternalData >();

        private readonly HlCompilerSettings m_Settings = SettingsManager.GetSettings < HlCompilerSettings >();
        private readonly string m_Directory;

        private readonly List < IncludedItem > m_IncludedFiles = new List < IncludedItem >();
        private readonly List < string > m_ImportedItems = new List < string >();
        private readonly HlCompilation m_Parent;

        private readonly Queue < string > m_UnusedTempVars = new Queue < string >();
        private readonly List < string > m_UsedTempVars = new List < string >();
        private readonly Scope < VariableData > m_VariableMap = new Scope < VariableData >();
        private string m_ParsedText;

        private readonly BuildDataStore m_DataStore;

        public event Action < string, string > OnCompiledIncludedScript;

        internal List < IExternalData > ExternalSymbols
        {
            get
            {
                if ( m_Parent != null )
                {
                    return m_Parent.ExternalSymbols.Concat( externalSymbols ).ToList();
                }

                return externalSymbols;
            }
            set => externalSymbols = value;
        }

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

        public HlCompilation(
            string originalText,
            string directory,
            BuildDataStore dataStore,
            HlNamespace root = null )
        {
            if ( root != null )
            {
                Root = root;
            }

            m_DataStore = dataStore;
            m_Directory = directory;

            TypeSystem = HlTypeSystem.Create( Root );
            TypeMap = new HlCompilerCollection( TypeSystem );
            OriginalText = TextImporter.ImportFile( originalText, directory );
        }

        public HlCompilation( HlCompilation parent, string id, HlNamespace root = null )
        {
            Root = root ?? parent.Root;
            TypeSystem = parent.TypeSystem;
            m_DataStore = parent.m_DataStore;
            OriginalText = parent.OriginalText;
            m_Directory = parent.m_Directory;
            m_VariableMap = new Scope < VariableData >( parent.m_VariableMap, id );
            ConstValTypes = new Scope < ConstantValueItem >( parent.ConstValTypes, id );
            FunctionMap = new Scope < FunctionData >( parent.FunctionMap, id );

            //m_IncludedFiles = new List < string >( parent.m_IncludedFiles );
            TypeSystem = parent.TypeSystem;
            m_Parent = parent;
            TypeMap = new HlCompilerCollection( TypeSystem );
        }

        public static void ResetCounter()
        {
            s_Counter = 0;
        }

        internal static string GetUniqueName( string prefix = null )
        {
            return ( prefix == null ? "" : prefix + "_" ) + s_Counter++;
        }

        public bool ContainsLocalVariable( string var )
        {
            return m_VariableMap.ContainsLocal( var );
        }

        public bool ContainsVariable( string var )
        {
            return m_VariableMap.Contains( var );
        }

        public void CreateVariable( string name, uint size, HlTypeDefinition tdef, VariableDataEmitFlags emFlags )
        {
            m_VariableMap.Set(
                name,
                new VariableData(
                    name,
                    m_VariableMap.GetFinalName( name ),
                    size,
                    tdef,
                    emFlags
                )
            );
        }

        public void CreateVariable( string name, string content, HlTypeDefinition tdef, VariableDataEmitFlags emFlags )
        {
            m_VariableMap.Set(
                name,
                new VariableData(
                    name,
                    m_VariableMap.GetFinalName( name ),
                    content,
                    tdef,
                    emFlags
                )
            );
        }

        public List < string > EmitVariables( bool printHead )
        {
            List < string > sb = new List < string >();

            if ( printHead )
            {
                sb.Add( "; ________________ VARIABLE FIELDS ________________" );
            }

            foreach ( KeyValuePair < string, VariableData > keyValuePair in m_VariableMap )
            {
                sb.Add( keyValuePair.Value.EmitVasm() );
            }

            return sb;
        }

        public string GetFinalName( string name )
        {
            if ( m_VariableMap.Contains( name ) )
            {
                return m_VariableMap.Get( name ).GetFinalName();
            }

            return m_VariableMap.GetFinalName( name );
        }

        public VariableData GetVariable( string name )
        {
            if ( m_VariableMap.Contains( name ) )
            {
                return m_VariableMap.Get( name );
            }

            EventManager < ErrorEvent >.SendEvent( new HlVariableNotFoundEvent( name, false ) );

            return new VariableData();
        }

        public bool HasFlag( HlLocalCompilationFlags flag )
        {
            return m_LocalCompilationFlags.Contains( flag.ToString() );
        }

        public string Parse( bool printHead = true, string appendAfterProg = "HLT" )
        {
            if ( m_ParsedText != null )
            {
                return m_ParsedText;
            }

            HlParserSettings hlpS = new HlParserSettings();

            List < IHlToken > tokens = PrepareForInline();
            ProcessImports();
            ParseDependencies();
            ParseFunctionToken( tokens, hlpS );
            ParseNamespaces( Root, tokens );
            TypeSystem.Finalize( this );
            ParseTypeDefinitions( TypeSystem, hlpS, tokens, Root );
            TypeSystem.Finalize( this );

            HlExpressionParser p = HlExpressionParser.Create( new HlExpressionReader( tokens ) );

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

        public void ParseFunctionToken(
            List < IHlToken > tokens,
            HlParserSettings settings,
            HlTypeDefinition tdef = null )
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                if ( tokens[i].Type == HlTokenType.OpBlockToken || tokens[i].Type == HlTokenType.OpSemicolon )
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

                    bool destructor = funcIdx - 1 >= 0 && tokens[funcIdx - 1].Type == HlTokenType.OpTilde;

                    IHlToken funcName = HlParsingTools.ReadOne( tokens, funcIdx, HlTokenType.OpWord );

                    IHlToken typeName = null;

                    if ( !destructor && funcIdx > 0 ||
                         destructor && funcIdx > 1 )
                    {
                        if ( !destructor && settings.MemberModifiers.ContainsValue( tokens[funcIdx - 1].Type ) ||
                             destructor && settings.MemberModifiers.ContainsValue( tokens[funcIdx - 2].Type ) )
                        {
                            int modStart = funcIdx - 1;

                            if ( destructor )
                            {
                                modStart--;
                            }

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
                                    new HlTextToken(
                                        HlTokenType.OpTypeVoid,
                                        "void",
                                        0
                                    ),
                                    ParseArgumentList( args.Reverse().ToList() ),
                                    mods,
                                    block.GetChildren().ToArray(),
                                    start,
                                    tdef,
                                    destructor
                                        ? HlFunctionType.Destructor
                                        : HlFunctionType.Constructor
                                )
                            );

                            i = start;
                        }
                        else if ( tokens[funcIdx - 1].Type == HlTokenType.OpWord ||
                                  tokens[funcIdx - 1].Type == HlTokenType.OpTypeVoid )
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
                            IHlToken block = null;

                            if ( tokens[i].Type == HlTokenType.OpBlockToken )
                            {
                                if ( mods.Any( x => x.Type == HlTokenType.OpAbstractMod ) )
                                {
                                    EventManager < ErrorEvent >.SendEvent(
                                        new HlTokenReadEvent(
                                            tokens,
                                            HlTokenType.OpSemicolon,
                                            HlTokenType.OpBlockToken,
                                            i
                                        )
                                    );
                                }
                                else
                                {
                                    block = tokens[i];
                                }
                            }

                            tokens.RemoveRange( start, end - start + 1 );

                            tokens.Insert(
                                start,
                                new FunctionDefinitionToken(
                                    funcName,
                                    typeName,
                                    ParseArgumentList( args.Reverse().ToList() ),
                                    mods,
                                    block?.GetChildren().ToArray(),
                                    start,
                                    tdef
                                )
                            );

                            i = start;
                        }
                    }
                    else
                    {
                        int start = funcIdx;
                        int end = i;
                        IHlToken block = tokens[i];

                        if ( block.Type == HlTokenType.OpSemicolon )
                        {
                            continue;
                        }

                        tokens.RemoveRange( start, end - start );

                        tokens.Insert(
                            start,
                            new FunctionDefinitionToken(
                                funcName,
                                new HlTextToken( HlTokenType.OpTypeVoid, "void", 0 ),
                                ParseArgumentList( args.Reverse().ToList() ),
                                new IHlToken[0],
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
                if (tokens[i].Type == HlTokenType.OpNumSign && tokens.Count > i + 2)
                {
                    if (tokens[i + 1].ToString() == "import" && tokens[i + 2].Type == HlTokenType.OpStringLiteral)
                    {
                        m_ImportedItems.Add(tokens[i + 2].ToString());
                        tokens.RemoveRange(i, 3);
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
                    if (tokens[i + 1].ToString() == "include" && tokens[i + 2].Type == HlTokenType.OpStringLiteral)
                    {
                        string c = UriResolver.GetFilePath(m_Directory, tokens[i + 2].ToString());
                        int j = i + 3;
                        List<string> extSyms = new List<string>();

                        while (j < tokens.Count && tokens[j].Type == HlTokenType.OpWord)
                        {
                            extSyms.Add(tokens[j].ToString());
                            j++;
                        }

                        m_IncludedFiles.Add(
                            new IncludedItem(c ?? tokens[i + 2].ToString(), false, extSyms.ToArray())
                        );

                        tokens.RemoveRange(i, 3 + extSyms.Count);
                    }
                    else if (tokens[i + 1].ToString() == "linclude" && tokens[i + 2].Type == HlTokenType.OpStringLiteral)
                    {
                        string c = UriResolver.GetFilePath(m_Directory, tokens[i + 2].ToString());
                        int j = i + 3;
                        List<string> extSyms = new List<string>();

                        while (j < tokens.Count && tokens[j].Type == HlTokenType.OpWord)
                        {
                            extSyms.Add(tokens[j].ToString());
                            j++;
                        }

                        m_IncludedFiles.Add(
                            new IncludedItem(c ?? tokens[i + 2].ToString(), false, extSyms.ToArray(), true)
                        );

                        tokens.RemoveRange(i, 3 + extSyms.Count);
                    }
                    else if ( tokens[i + 1].ToString() == "inline" &&
                              tokens[i + 2].Type == HlTokenType.OpStringLiteral )
                    {
                        string c = UriResolver.GetFilePath( m_Directory, tokens[i + 2].ToString() );
                        m_IncludedFiles.Add( new IncludedItem( c ?? tokens[i + 2].ToString(), true ) );
                        tokens.RemoveRange( i, 3 );
                    }
                }
            }
        }

        public void ParseLocalFlags( List < IHlToken > tokens )
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                if ( tokens[i].Type == HlTokenType.OpNumSign && tokens.Count > i + 2 )
                {
                    if ( tokens[i + 1].ToString() == "enable" && tokens[i + 2].Type == HlTokenType.OpWord )
                    {
                        m_LocalCompilationFlags.Add( tokens[i + 2].ToString() );
                        tokens.RemoveRange( i, 3 );
                    }
                }
            }
        }

        public void ParseNamespaces( HlNamespace root, List < IHlToken > tokens )
        {
            for ( int i = tokens.Count - 1; i >= 0; i-- )
            {
                if ( tokens[i].Type == HlTokenType.OpBlockToken )
                {
                    if ( i == 0 )
                    {
                        continue;
                    }

                    int nsIndex = tokens.FindLastIndex( i - 1, x => x.Type == HlTokenType.OpNamespace );

                    if ( nsIndex == -1 )
                    {
                        continue;
                    }

                    int read = HlParsingTools.ReadList(
                        tokens,
                        i - 1,
                        HlTokenType.OpWord,
                        HlTokenType.OpNamespaceSeparator,
                        out IHlToken[] nameParts
                    );

                    if ( nsIndex != i - 1 - read )
                    {
                        continue;
                    }

                    HlNamespace ns = root.AddRecursive( nameParts.Select( x => x.ToString() ).ToArray() );

                    int start = tokens[i - 1 - read].SourceIndex;
                    IHlToken block = tokens[i];
                    tokens.RemoveRange( i - 1 - read, read + 2 );

                    tokens.Insert(
                        i - 1 - read,
                        new NamespaceDefinitionToken( ns, block.GetChildren().ToArray(), start )
                    );

                    i = i - 2 - read;
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

        public List < IHlToken > PrepareForInline()
        {
            HlParserSettings hlpS = new HlParserSettings();
            HlParserBaseReader br = new HlParserBaseReader( hlpS, OriginalText );

            List < IHlToken > tokens = br.ReadToEnd();
            ParseOneLineStrings( tokens );
            ParseCharTokens( tokens );
            RemoveComments( tokens );
            ParseImports( tokens );
            ParseLocalFlags( tokens );
            ParseIncludes( tokens );
            ParseReservedKeys( tokens, hlpS );
            tokens = tokens.Where( x => x.Type != HlTokenType.OpNewLine ).ToList();
            ParseVarDefToken( tokens, hlpS );
            ParseBlocks( tokens );

            ParseInLineScripts( tokens );

            return tokens;
        }

        public void Unload()
        {
            OriginalText = null;
            externalSymbols.Clear();
            m_IncludedFiles.Clear();
            m_UnusedTempVars.Clear();
            m_UsedTempVars.Clear();
            m_ParsedText = null;
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

            foreach ( IncludedItem includedFile in m_IncludedFiles )
            {
                sb.AppendLine( $":include \"{includedFile.Data}\"" );
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

            EmitVariables( printHead ).ForEach( x => sb.AppendLine( x ) );

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

        private static string[] CompileMemberFunction(
            HlCompilation compilation,
            HlCompilation subCompilation,
            HlFunctionType type,
            string funcName,
            bool isStatic,
            HlTypeDefinition tdef,
            HlFuncDefOperand fdef )
        {
            if ( !isStatic || type != HlFunctionType.Function )
            {
                subCompilation.CreateVariable(
                    "this",
                    1,
                    tdef,
                    VariableDataEmitFlags.Pointer
                );
            }

            foreach ( IHlToken valueArgument in fdef.FunctionDefinition.
                                                     Arguments )
            {
                VariableDefinitionToken vdef =
                    valueArgument as VariableDefinitionToken;

                string key = vdef.Name.ToString();

                subCompilation.CreateVariable(
                    key,
                    1,
                    compilation.TypeSystem.GetType(
                        compilation.Root,
                        vdef.TypeName.ToString()
                    ),
                    VariableDataEmitFlags.None
                );
            }

            if ( fdef.FunctionDefinition.FunctionType == HlFunctionType.Constructor )
            {
                subCompilation.EmitterResult.Store(
                    "." +
                    tdef.GetInternalConstructor( compilation ) +
                    ( fdef.FunctionDefinition.Mods.Any(
                        x => x.Type == HlTokenType.OpPublicMod
                    )
                        ? ""
                        : " linker:hide" )
                );
            }

            if ( fdef.FunctionDefinition.FunctionType == HlFunctionType.Constructor &&
                 SettingsManager.GetSettings < HlCompilerSettings >().ConstructorPrologMode ==
                 HlTypeConstructorPrologMode.Inline )

            {
                InvocationExpressionCompiler.WriteInlineConstructorInvocationProlog( subCompilation, tdef, fdef );
            }

            subCompilation.EmitterResult.Store(
                "." +
                funcName +
                ( fdef.FunctionDefinition.Mods.Any(
                    x => x.Type == HlTokenType.OpPublicMod
                )
                    ? ""
                    : " linker:hide" )
            );

            for ( int i = fdef.FunctionDefinition.
                               Arguments.Length -
                          1;
                  i >= 0;
                  i-- )
            {
                IHlToken valueArgument = fdef.FunctionDefinition.
                                              Arguments[i];

                subCompilation.EmitterResult.Emit(
                    $"POP",
                    $"{subCompilation.GetFinalName( ( valueArgument as VariableDefinitionToken ).Name.ToString() )}"
                );
            }

            if ( !isStatic || type != HlFunctionType.Function )
            {
                subCompilation.EmitterResult.Emit( $"POP", $"{subCompilation.GetFinalName( "this" )}" );
            }

            List < string > parsedVal =
                subCompilation.Parse( fdef.Block, false, null ).
                               Replace( "\r", "" ).
                               Split( '\n' ).
                               ToList();

            parsedVal.Add( $"PUSH 0" );

            parsedVal.Add( "RET" );

            return parsedVal.ToArray();
        }

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

            Dictionary < string, string[] > preParsedData = new Dictionary < string, string[] >();

            foreach ( KeyValuePair < string, FunctionData > keyValuePair in FunctionMap )
            {
                preParsedData[keyValuePair.Key] = keyValuePair.Value.GetCompiledOutput();
            }

            foreach ( KeyValuePair < string, FunctionData > keyValuePair in FunctionMap )
            {
                if ( !HasFlag( HlLocalCompilationFlags.HL_OPT_NO_STRIP ) )
                {
                    if ( m_Settings.StripUnusedFunctions && keyValuePair.Value.UseCount == 0 )
                    {
                        if ( HasFlag( HlLocalCompilationFlags.HL_OPT_STRIP_PUBLIC ) || !keyValuePair.Value.Public )
                        {

                            Log(
                                "Stripping '{0}'('{1}') from build output",
                                keyValuePair.Value.GetName(),
                                keyValuePair.Value.GetFinalName() );

                            continue;
                        }
                    }
                }

                string[] funcOutputData = preParsedData[keyValuePair.Key];

                if ( funcOutputData.Length == 0 )
                {
                    Log(
                        "Ignoring Empty Function '{0}'('{1}')",
                        keyValuePair.Value.GetName(),
                        keyValuePair.Value.GetFinalName() );

                    continue;
                }

                if ( m_Parent == null )
                {
                    //sb.AppendLine("." + keyValuePair.Key + (keyValuePair.Value.Public ? "" : " linker:hide"));

                    foreach ( string s in funcOutputData )
                    {
                        sb.AppendLine( s );
                    }
                }
                else if ( !m_Parent.FunctionMap.Contains( keyValuePair.Key ) )
                {
                    m_Parent.FunctionMap.Set( keyValuePair.Key, keyValuePair.Value );
                }
            }
        }

        private VariableData GetFreeTempVar( uint initValue )
        {
            if ( m_UnusedTempVars.Count != 0 )
            {
                string oldName = m_UnusedTempVars.Dequeue();

                EmitterResult.Emit( "LOAD", oldName, initValue.ToString() );

                return m_VariableMap.Get( oldName );
            }

            string name = GetUniqueName( "tmp" );

            //if ( initValue != 0 )
            //{
            EmitterResult.Emit( $"LOAD", name, initValue.ToString() );

            //}

            return m_VariableMap.Set(
                name,
                new VariableData(
                    name,
                    name,
                    1,
                    TypeSystem.GetType( Root, HLBaseTypeNames.s_UintTypeName ),
                    VariableDataEmitFlags.None
                )
            );
        }

        private VariableData GetFreeTempVarCopy( string initValue )
        {
            if ( m_UnusedTempVars.Count != 0 )
            {
                string oldName = m_UnusedTempVars.Dequeue();
                EmitterResult.Emit( $"COPY", initValue, oldName );

                return m_VariableMap.Get( oldName );
            }

            string name = GetUniqueName( "tmp" );

            EmitterResult.Emit( $"COPY", initValue, name );

            return m_VariableMap.Set(
                name,
                new VariableData(
                    name,
                    name,
                    1,
                    TypeSystem.GetType( Root, HLBaseTypeNames.s_UintTypeName ),
                    VariableDataEmitFlags.None
                )
            );
        }

        private VariableData GetFreeTempVarDref( string initValue )
        {
            if ( m_UnusedTempVars.Count != 0 )
            {
                string oldName = m_UnusedTempVars.Dequeue();
                EmitterResult.Emit( $"DREF", initValue, oldName );

                return m_VariableMap.Get( oldName );
            }

            string name = GetUniqueName( "tmp" );

            EmitterResult.Emit( $"DREF", initValue, name );

            return m_VariableMap.Set(
                name,
                new VariableData(
                    name,
                    name,
                    1,
                    TypeSystem.GetType( Root, HLBaseTypeNames.s_UintTypeName ),
                    VariableDataEmitFlags.None
                )
            );
        }

        private VariableData GetFreeTempVarLoad( string initValue )
        {
            if ( m_UnusedTempVars.Count != 0 )
            {
                string oldName = m_UnusedTempVars.Dequeue();
                EmitterResult.Emit( $"LOAD", oldName, initValue );

                return m_VariableMap.Get( oldName );
            }

            string name = GetUniqueName( "tmp" );

            EmitterResult.Emit( $"LOAD", name, initValue );

            return m_VariableMap.Set(
                name,
                new VariableData(
                    name,
                    name,
                    1,
                    TypeSystem.GetType( Root, HLBaseTypeNames.s_UintTypeName ),
                    VariableDataEmitFlags.None
                )
            );
        }

        private VariableData GetFreeTempVarPop()
        {
            if ( m_UnusedTempVars.Count != 0 )
            {
                string oldName = m_UnusedTempVars.Dequeue();
                EmitterResult.Emit( "POP", oldName );

                return m_VariableMap.Get( oldName );
            }

            string name = GetUniqueName( "tmp" );

            EmitterResult.Emit( "POP", name );

            return m_VariableMap.Set(
                name,
                new VariableData(
                    name,
                    name,
                    1,
                    TypeSystem.GetType( Root, HLBaseTypeNames.s_UintTypeName ),
                    VariableDataEmitFlags.None
                )
            );
        }

        private void ParseDependencies()
        {
            for ( int i = 0; i < m_IncludedFiles.Count; i++ )
            {
                IncludedItem includedFile = m_IncludedFiles[i];

                if ( m_Parent != null && m_Parent.m_IncludedFiles.Contains( includedFile ) )
                {
                    continue;
                }

                if ( includedFile.Data.EndsWith( ".vasm" ) )
                {
                    Log( "Including VASM File: {0}", includedFile.Data );

                    string name = Path.GetFullPath(
                        includedFile.Data.StartsWith( m_Directory )
                            ? includedFile.Data.Remove( includedFile.Data.Length - 4, 4 )
                            : m_Directory +
                              "/" +
                              includedFile.Data.Remove( includedFile.Data.Length - 4, 4 )
                    );

                    UriKind kind = includedFile.Data.StartsWith( "/" ) || includedFile.Data.StartsWith( "\\" )
                        ? UriKind.Absolute
                        : UriKind.RelativeOrAbsolute;

                    Uri import = null;

                    if ( kind == UriKind.Absolute )
                    {
                        import = new Uri( "file://" + includedFile, kind );
                    }
                    else
                    {
                        import = new Uri( includedFile.Data, kind );
                    }

                    Uri dir = new Uri( "file://" + Directory.GetCurrentDirectory() + "/", UriKind.Absolute );

                    if ( import.IsAbsoluteUri )
                    {
                        name = dir.MakeRelativeUri( import ).OriginalString;
                        name = name.Remove( name.Length - 4, 4 );
                    }

                    string newInclude = m_DataStore.GetStorePath( "HL2VASM", name );
                    string file = Path.GetFullPath( name + ".vasm" );

                    includedFile = new IncludedItem( newInclude, includedFile.IsInline, includedFile.ExternalSymbols );
                    File.Copy( file, newInclude );

                    foreach ( string includedFileExternalSymbol in includedFile.ExternalSymbols )
                    {
                        externalSymbols.Add(
                            new VariableData(
                                includedFileExternalSymbol,
                                includedFileExternalSymbol,
                                1,
                                TypeSystem.GetType( Root, HLBaseTypeNames.s_UintTypeName ),
                                VariableDataEmitFlags.Visible,
                                ExternalDataType.Function
                            )
                        );
                    }
                }
                else if ( includedFile.Data.EndsWith( ".vhl" ) )
                {
                    Log( "Including File: {0}", includedFile.Data );

                    string name = Path.GetFullPath(
                        includedFile.Data.StartsWith( m_Directory )
                            ? includedFile.Data.Remove( includedFile.Data.Length - 4, 4 )
                            : m_Directory +
                              "/" +
                              includedFile.Data.Remove( includedFile.Data.Length - 4, 4 )
                    );

                    UriKind kind = includedFile.Data.StartsWith( "/" ) || includedFile.Data.StartsWith( "\\" )
                        ? UriKind.Absolute
                        : UriKind.RelativeOrAbsolute;

                    Uri import = null;

                    if ( kind == UriKind.Absolute )
                    {
                        import = new Uri( "file://" + includedFile, kind );
                    }
                    else
                    {
                        import = new Uri( includedFile.Data, kind );
                    }

                    Uri dir = new Uri( "file://" + Directory.GetCurrentDirectory() + "/", UriKind.Absolute );

                    if ( import.IsAbsoluteUri )
                    {
                        name = dir.MakeRelativeUri( import ).OriginalString;
                        name = name.Remove( name.Length - 4, 4 );
                    }

                    string newInclude = m_DataStore.GetStorePath( "HL2VASM", name );
                    string file = Path.GetFullPath( name + ".vhl" );
                    string srcContent = File.ReadAllText( file );

                    HlCompilation comp = null;
                    bool cached = false;

                    if ( !HasFlag( HlLocalCompilationFlags.HL_COMP_NO_CACHE ) &&
                         File.Exists( newInclude ) &&
                         HlCompilerCache.HasCached( srcContent ) )
                    {
                        comp = HlCompilerCache.Get( srcContent );
                        cached = true;
                    }
                    else
                    {
                        comp = new HlCompilation(
                            srcContent,
                            Path.GetDirectoryName( file ),
                            m_DataStore
                        );

                        if ( HasFlag( HlLocalCompilationFlags.HL_LOC_MAKE_GLOBAL ) )
                        {
                            comp.m_LocalCompilationFlags.AddRange( m_LocalCompilationFlags );
                        }

                        File.WriteAllText( newInclude, comp.Parse() );
                    }

                    TypeSystem.Import( Root, comp.TypeSystem );

                    externalSymbols.AddRange(
                        comp.ConstValTypes.Where( x => x.Value.IsPublic ).
                             Select(
                                 x => new ConstantData(
                                     x.Key,
                                     x.Key,
                                     x.Value.Value,
                                     TypeSystem.GetType(
                                         Root,
                                         HLBaseTypeNames.s_UintTypeName
                                     ),
                                     true
                                 )
                             ).
                             Cast < IExternalData >()
                    );

                    externalSymbols.AddRange(
                        comp.FunctionMap.
                             Where(
                                 x => x.Value.Public &&
                                      externalSymbols.All(
                                          y => y.GetFinalName() != x.Value.GetFinalName()
                                      )
                             ).
                             Select( x => x.Value )
                    );

                    externalSymbols.AddRange(
                        comp.externalSymbols.Where(
                            x => externalSymbols.All(
                                y => y.GetFinalName() != x.GetFinalName()
                            )
                        )
                    );

                    includedFile = new IncludedItem( newInclude, includedFile.IsInline );

                    foreach ( IncludedItem compIncludedFile in comp.m_IncludedFiles )
                    {
                        if (!compIncludedFile.IsLocal && !m_IncludedFiles.Contains( compIncludedFile ) )
                        {
                            m_IncludedFiles.Insert( 0, compIncludedFile );
                            i++;
                        }
                    }

                    if ( !cached )
                    {
                        HlCompilerCache.Add( srcContent, comp );
                    }
                    else
                    {
                        comp.Unload();
                    }
                }

                OnCompiledIncludedScript?.Invoke(
                    Path.GetFullPath( m_Directory + "/" + m_IncludedFiles[i].Data ),
                    includedFile.Data
                );

                m_IncludedFiles[i] = includedFile;
            }
        }

        private void ParseInLineScripts( List < IHlToken > tokens )
        {
            for ( int i = m_IncludedFiles.Count - 1; i >= 0; i-- )
            {
                IncludedItem includedFile = m_IncludedFiles[i];

                if ( m_Parent != null && m_Parent.m_IncludedFiles.Contains( includedFile ) )
                {
                    continue;
                }

                if ( includedFile.IsInline && includedFile.Data.EndsWith( ".vhl" ) )
                {
                    Log( "Inlining File: {0}", includedFile.Data );

                    string name = Path.GetFullPath(
                        includedFile.Data.StartsWith( m_Directory )
                            ? includedFile.Data.Remove( includedFile.Data.Length - 4, 4 )
                            : m_Directory +
                              "/" +
                              includedFile.Data.Remove( includedFile.Data.Length - 4, 4 )
                    );

                    UriKind kind = includedFile.Data.StartsWith( "/" ) || includedFile.Data.StartsWith( "\\" )
                        ? UriKind.Absolute
                        : UriKind.RelativeOrAbsolute;

                    Uri import = null;

                    if ( kind == UriKind.Absolute )
                    {
                        import = new Uri( "file://" + includedFile, kind );
                    }
                    else
                    {
                        import = new Uri( includedFile.Data, kind );
                    }

                    Uri dir = new Uri( "file://" + Directory.GetCurrentDirectory() + "/", UriKind.Absolute );

                    if ( import.IsAbsoluteUri )
                    {
                        name = dir.MakeRelativeUri( import ).OriginalString;
                        name = name.Remove( name.Length - 4, 4 );
                    }

                    string file = Path.GetFullPath( name + ".vhl" );
                    string srcContent = File.ReadAllText( file );

                    HlCompilation comp = new HlCompilation(
                        srcContent,
                        Path.GetDirectoryName( file ),
                        m_DataStore
                    );

                    tokens.AddRange( comp.PrepareForInline() );

                    comp.Unload();

                    m_IncludedFiles.RemoveAt( i );
                }

                OnCompiledIncludedScript?.Invoke(
                    Path.GetFullPath( m_Directory + "/" + m_IncludedFiles[i].Data ),
                    includedFile.Data
                );
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
            List < IHlToken > block,
            HlParserSettings settings )
        {
            ParseFunctionToken( block, settings, tdef );
            HlExpressionParser p = HlExpressionParser.Create( new HlExpressionReader( block ) );
            HlExpression[] exprs = p.Parse();

            foreach ( HlExpression hlToken in exprs )
            {
                if ( hlToken is HlVarDefOperand t )
                {
                    HlTypeDefinition tt = ts.GetType( Root, t.VariableDefinition.TypeName.ToString() );

                    uint arrSize = t.VariableDefinition.Size?.ToString().ParseUInt() ?? 1;

                    if ( t.VariableDefinition.Size != null )
                    {
                        tt = new ArrayTypeDefintion(
                            Root,
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

                    if ( t.VariableDefinition.Modifiers.Any(
                        x => x.Type == HlTokenType.OpStaticMod ||
                             x.Type == HlTokenType.OpConstMod
                    ) )
                    {
                        string asmVarName = tdef.GetFinalStaticProperty( t.VariableDefinition.Name.ToString() );

                        CreateVariable(
                            asmVarName,
                            arrSize,
                            tt,
                            t.VariableDefinition.Modifiers.Any(
                                x => x.Type == HlTokenType.OpPublicMod
                            )
                                ? VariableDataEmitFlags.Visible
                                : VariableDataEmitFlags.None
                        );

                        HlExpression init = t.Initializer.FirstOrDefault();

                        if ( init != null )
                        {
                            if ( init is HlValueOperand vOp &&
                                 vOp.Value.Type == HlTokenType.OpStringLiteral )
                            {
                                string content = vOp.Value.ToString();
                                CreateVariable( asmVarName, content, tt, VariableDataEmitFlags.None );
                            }
                        }
                    }
                }
                else if ( hlToken is HlFuncDefOperand fdef )
                {
                    HlFunctionDefinition funcDef = new HlFunctionDefinition(
                        fdef.FunctionDefinition.FunctionName.
                             ToString(),
                        ts.GetType(
                            Root,
                            fdef.FunctionDefinition.
                                 FunctionReturnType.
                                 ToString()
                        ),
                        fdef.FunctionDefinition.Arguments.
                             Select(
                                 x => ts.GetType(
                                     Root,
                                     x.GetChildren().
                                       First().
                                       ToString()
                                 )
                             ).
                             ToArray(),
                        fdef.FunctionDefinition.Mods
                    );

                    tdef.AddMember(
                        funcDef
                    );

                    string funcName =
                        tdef.GetFinalMemberName( funcDef ); //$"FUN_{tdef.Name}_{fdef.FunctionDefinition.FunctionName}";

                    HlCompilation fComp = new HlCompilation( this, funcName, tdef.Namespace );

                    bool isStatic = fdef.FunctionDefinition.Mods.Any(
                        x => x.Type == HlTokenType.OpStaticMod
                    );

                    bool isAbstract = fdef.FunctionDefinition.Mods.Any(
                        x => x.Type == HlTokenType.OpAbstractMod
                    );

                    Func < string[] > compiler = null;

                    if ( !isAbstract )
                    {
                        compiler = () => CompileMemberFunction(
                            this,
                            fComp,
                            fdef.FunctionDefinition.FunctionType,
                            funcName,
                            isStatic,
                            tdef,
                            fdef
                        );
                    }

                    FunctionMap.Set(
                        funcName,
                        new FunctionData(
                            funcName,
                            fdef.FunctionDefinition.Mods.Any(
                                x => x.Type == HlTokenType.OpPublicMod
                            ),
                            isStatic,
                            compiler,
                            fdef.FunctionDefinition.Arguments.Length,
                            fdef.FunctionDefinition.FunctionReturnType.ToString()
                        )
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

            if ( tdef.StaticConstructor == null )
            {
                HlFunctionDefinition sctor = new HlFunctionDefinition(
                    tdef.Name,
                    TypeSystem.GetType( Root, "void" ),
                    new HlTypeDefinition[0],
                    new[]
                    {
                        new HlTextToken(
                            HlTokenType.OpPublicMod,
                            "public",
                            -1
                        ),
                        new HlTextToken(
                            HlTokenType.OpStaticMod,
                            "static",
                            -1
                        )
                    }
                );

                tdef.AddMember( sctor );

                string funcName = tdef.GetFinalMemberName( sctor );
                HlCompilation fComp = new HlCompilation( this, funcName, tdef.Namespace );

                Func < string[] > compiler = () =>
                {
                    fComp.CreateVariable(
                        "this",
                        1,
                        tdef,
                        VariableDataEmitFlags.Pointer
                    );

                    fComp.EmitterResult.Emit( "POP", fComp.GetFinalName( "this" ) );

                    InvocationExpressionCompiler.WriteConstructorInvocationProlog(
                        fComp,
                        tdef,
                        fComp.GetFinalName( "this" )
                    );

                    List < string > parsedVal = fComp.EmitterResult.Get().ToList();
                    parsedVal.Insert( 0, "." + tdef.GetInternalConstructor( fComp ) );
                    parsedVal.InsertRange( 0, fComp.EmitVariables( false ) );

                    parsedVal.Add( "." + funcName );
                    parsedVal.Add( "PUSH 0" );
                    parsedVal.Add( "RET" );

                    return parsedVal.ToArray();
                };

                FunctionMap.Set(
                    funcName,
                    new FunctionData(
                        funcName,
                        true,
                        true,
                        compiler,
                        1,
                        "void"
                    )
                );
            }
        }

        private void ParseTypeDefinitions(
            HlTypeSystem ts,
            HlParserSettings settings,
            List < IHlToken > tokens,
            HlNamespace current )
        {
            for ( int i = tokens.Count - 1; i >= 0; i-- )
            {
                if ( i >= tokens.Count )
                {
                    continue;
                }

                if ( tokens[i].Type == HlTokenType.OpBlockToken )
                {
                    ParseTypeDefinitions( ts, settings, tokens[i].GetChildren(), current );

                    continue;
                }

                if ( tokens[i].Type == HlTokenType.OpNamespaceDefinition )
                {
                    ParseTypeDefinitions(
                        ts,
                        settings,
                        tokens[i].GetChildren(),
                        ( tokens[i] as NamespaceDefinitionToken ).Namespace
                    );

                    tokens.RemoveAt( i );

                    continue;
                }

                if ( tokens[i].Type == HlTokenType.OpClass )
                {
                    IHlToken classType = tokens[i];
                    IHlToken name = HlParsingTools.ReadOne( tokens, i + 1, HlTokenType.OpWord );

                    IHlToken[] mods = HlParsingTools.ReadNoneOrManyOf(
                                                         tokens,
                                                         i - 1,
                                                         -1,
                                                         settings.ClassModifiers.Values.ToArray()
                                                     ).
                                                     Reverse().
                                                     ToArray();

                    IHlToken[] baseClasses = new IHlToken[0];
                    int offset = 2;

                    if ( HlParsingTools.ReadOneOrNone(
                        tokens,
                        i + offset,
                        HlTokenType.OpColon,
                        out IHlToken inhColon
                    ) )
                    {
                        int read = HlParsingTools.ReadList(
                            tokens,
                            i + offset + 1,
                            HlTokenType.OpWord,
                            HlTokenType.OpComma,
                            out baseClasses
                        );

                        offset += read + 1;
                    }

                    IHlToken block = HlParsingTools.ReadOne(
                        tokens,
                        i + offset,
                        HlTokenType.OpBlockToken
                    );

                    int start = i - mods.Length;
                    int end = i + offset + 1;
                    tokens.RemoveRange( start, end - start );

                    if ( m_Parent != null && m_Parent.TypeSystem.HasType( Root, name.ToString() ) )
                    {
                        continue;
                    }

                    HlTypeDefinition def = ts.CreateEmptyType(
                        current,
                        name.ToString(),
                        mods.Any( x => x.Type == HlTokenType.OpPublicMod ),
                        mods.Any( x => x.Type == HlTokenType.OpAbstractMod ),
                        classType.ToString() == "struct"
                    );

                    foreach ( IHlToken baseClass in baseClasses )
                    {
                        def.AddBaseType( baseClass );
                    }

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
                    IExternalData[] data = dimp.ProcessImport( this, m_ImportedItems[i] );

                    externalSymbols.AddRange( data );
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
                        idx = tokens.Count;
                    }

                    tokens.RemoveRange( i, idx - i );
                }
            }
        }

        #endregion
    }

}
