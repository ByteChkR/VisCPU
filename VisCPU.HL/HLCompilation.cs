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
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Events;
using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.Parser.Tokens.Combined;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.TypeSystem;
using VisCPU.Instructions.Emit;
using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.IO;
using VisCPU.Utility.Logging;
using VisCPU.Utility.Settings;
using VisCPU.Utility.SharedBase;
using VisCPU.Utility.UriResolvers;

namespace VisCPU.HL
{

    public class HLCompilation : VisBase
    {

        public readonly HLTypeSystem TypeSystem = new HLTypeSystem();
        internal const string STRING_TYPE = "string";
        internal const string VAL_TYPE = "var";
        internal readonly Dictionary<string, string> ConstValTypes = new Dictionary<string, string>();
        internal readonly List<IExternalData> ExternalSymbols = new List<IExternalData>();
        internal readonly Dictionary<string, FunctionData> FunctionMap = new Dictionary<string, FunctionData>();

        internal readonly string OriginalText;

        internal readonly EmitterResult<string> EmitterResult = new EmitterResult<string>(new TextEmitter());

        internal HLCompilerCollection TypeMap;

        private static uint s_Counter;

        private readonly HLCompilerSettings m_Settings = SettingsManager.GetSettings<HLCompilerSettings>();
        private readonly string m_Directory;

        private readonly List<string> m_IncludedFiles = new List<string>();
        private readonly List<string> m_ImportedItems = new List<string>();
        private readonly HLCompilation m_Parent;
        private readonly string m_ScopeId = "_";

        private readonly Queue<string> m_UnusedTempVars = new Queue<string>();
        private readonly List<string> m_UsedTempVars = new List<string>();
        private readonly Dictionary<string, VariableData> m_VariableMap = new Dictionary<string, VariableData>();
        private string m_ParsedText;

        private readonly BuildDataStore m_DataStore;

        public event Action<string, string> OnCompiledIncludedScript;

        protected override LoggerSystems SubSystem => LoggerSystems.HlCompiler;

        #region Public

        public HLCompilation(string originalText, string directory) : this(
                                                                             originalText,
                                                                             directory,
                                                                             new BuildDataStore(
                                                                                  directory,
                                                                                  new HLBuildDataStore()
                                                                                 )
                                                                            )
        {
        }

        public HLCompilation(string originalText, string directory, BuildDataStore dataStore)
        {
            m_DataStore = dataStore;
            OriginalText = originalText;
            m_Directory = directory;
            TypeMap = new HLCompilerCollection(TypeSystem);
        }

        public HLCompilation(HLCompilation parent, string id)
        {
            m_DataStore = parent.m_DataStore;
            m_ScopeId = id;
            OriginalText = parent.OriginalText;
            m_Directory = parent.m_Directory;
            m_VariableMap = new Dictionary<string, VariableData>(parent.m_VariableMap);
            ConstValTypes = new Dictionary<string, string>(parent.ConstValTypes);
            FunctionMap = new Dictionary<string, FunctionData>(parent.FunctionMap);
            ExternalSymbols = new List<IExternalData>(parent.ExternalSymbols);
            m_IncludedFiles = new List<string>(parent.m_IncludedFiles);
            TypeSystem = parent.TypeSystem;
            m_Parent = parent;
            TypeMap = new HLCompilerCollection(TypeSystem);
        }

        public static void ResetCounter()
        {
            s_Counter = 0;
        }

        internal static string GetUniqueName(string prefix = null)
        {
            return (prefix == null ? "" : prefix + "_") + s_Counter++;
        }

        public bool ContainsLocalVariable(string var)
        {
            return m_VariableMap.ContainsKey(GetFinalName(var));
        }

        public bool ContainsVariable(string var)
        {
            return m_VariableMap.ContainsKey(GetFinalName(var)) ||
                   m_Parent != null && m_Parent.ContainsVariable(var);
        }

        public void CreateVariable(string name, uint size, HLTypeDefinition tdef, bool isVisible)
        {
            m_VariableMap[GetFinalName(name)] = new VariableData(name, GetFinalName(name), size, tdef, isVisible);
        }

        public void CreateVariable(string name, string content, HLTypeDefinition tdef, bool isVisible)
        {
            m_VariableMap[GetFinalName(name)] =
                new VariableData(name, GetFinalName(name), content, tdef, isVisible);
        }

        public string GetFinalName(string name)
        {
            return GetPrefix() + name;
        }

        public VariableData GetVariable(string name)
        {
            if (m_VariableMap.ContainsKey(GetFinalName(name)))
            {
                return m_VariableMap[GetFinalName(name)];
            }

            if (m_Parent == null)
            {
                EventManager<ErrorEvent>.SendEvent(new HLVariableNotFoundEvent(name, false));

                return new VariableData();
            }

            return m_Parent.GetVariable(name);
        }

        public string Parse(bool printHead = true, string appendAfterProg = "HLT")
        {
            if (m_ParsedText != null)
            {
                return m_ParsedText;
            }

            HLParserSettings hlpS = new HLParserSettings();
            HLParserBaseReader br = new HLParserBaseReader(hlpS, OriginalText);

            List<IHlToken> tokens = br.ReadToEnd();
            ParseOneLineStrings(tokens);
            ParseCharTokens(tokens);
            RemoveComments(tokens);
            ParseImports(tokens);
            ParseIncludes(tokens);
            ParseReservedKeys(tokens, hlpS);
            tokens = tokens.Where(x => x.Type != HLTokenType.OpNewLine).ToList();
            ParseVarDefToken(tokens, hlpS);
            ParseBlocks(tokens);

            ParseFunctionToken(tokens, hlpS);
            ParseTypeDefinitions(TypeSystem, hlpS, tokens);

            HLExpressionParser p = HLExpressionParser.Create(new HLExpressionReader(tokens));
            ProcessImports();
            ParseDependencies();

            return Parse(p.Parse(), printHead, appendAfterProg);
        }

        public void ParseBlocks(List<IHlToken> tokens)
        {
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                if (tokens[i].Type == HLTokenType.OpBlockBracketClose)
                {
                    int current = 1;
                    int start = i - 1;

                    for (; start >= 0; start--)
                    {
                        if (tokens[start].Type == HLTokenType.OpBlockBracketClose)
                        {
                            current++;
                        }
                        else if (tokens[start].Type == HLTokenType.OpBlockBracketOpen)
                        {
                            current--;

                            if (current == 0)
                            {
                                break;
                            }
                        }
                    }

                    List<IHlToken> content = tokens.GetRange(start + 1, i - start - 1).ToList();
                    tokens.RemoveRange(start, i - start + 1);
                    ParseBlocks(content);
                    tokens.Insert(start, new BlockToken(content.ToArray(), start));
                    i = start;
                }
            }
        }

        public void ParseCharTokens(List<IHlToken> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (i < tokens.Count - 1 &&
                     tokens[i].Type == HLTokenType.OpSingleQuote)
                {
                    int idx = tokens.FindIndex(i + 1, t => t.Type == HLTokenType.OpNewLine);
                    int endQuote = tokens.FindIndex(i + 1, t => t.Type == HLTokenType.OpSingleQuote);

                    if (idx == -1)
                    {
                        idx = tokens.Count - 1;
                    }

                    if (endQuote == -1 || endQuote > idx)
                    {
                        EventManager<ErrorEvent>.SendEvent(
                                                              new HLTokenReadEvent(
                                                                   HLTokenType.OpSingleQuote,
                                                                   HLTokenType.OpNewLine
                                                                  )
                                                             );

                        return;
                    }

                    string ConcatContent()
                    {
                        List<IHlToken> content = tokens.GetRange(i + 1, endQuote - i - 1);

                        return OriginalText.Substring(
                                                      content.First().SourceIndex,
                                                      tokens[i + 1 + content.Count].SourceIndex -
                                                      content.First().SourceIndex
                                                     );
                    }

                    IHlToken newToken = new HLTextToken(
                                                        HLTokenType.OpCharLiteral,
                                                        ConcatContent(),
                                                        tokens[i].SourceIndex
                                                       );

                    tokens.RemoveRange(i, endQuote - i + 1);
                    tokens.Insert(i, newToken);
                }
            }
        }

        public void ParseFunctionToken(List<IHlToken> tokens, HLParserSettings settings)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == HLTokenType.OpBlockToken)
                {
                    if (!HLParsingTools.ReadOneOrNone(
                                                       tokens,
                                                       i - 1,
                                                       HLTokenType.OpBracketClose,
                                                       out IHlToken bClose
                                                      ))
                    {
                        continue;
                    }

                    List<IHlToken> argPart = new List<IHlToken> { bClose };
                    IHlToken[] args = HLParsingTools.ReadUntil(tokens, i - 2, -1, HLTokenType.OpBracketOpen);
                    argPart.AddRange(args);

                    IHlToken argOpenBracket = HLParsingTools.ReadOne(
                                                                     tokens,
                                                                     i - 2 - args.Length,
                                                                     HLTokenType.OpBracketOpen
                                                                    );

                    argPart.Add(argOpenBracket);
                    argPart.Reverse();

                    int funcIdx = i - 3 - args.Length;

                    if (tokens[funcIdx].Type != HLTokenType.OpWord)
                    {
                        continue;
                    }

                    IHlToken funcName = HLParsingTools.ReadOne(tokens, funcIdx, HLTokenType.OpWord);

                    IHlToken typeName = null;

                    if (funcIdx > 0 &&
                         (tokens[funcIdx - 1].Type == HLTokenType.OpWord ||
                           tokens[funcIdx - 1].Type == HLTokenType.OpTypeVoid))
                    {
                        typeName = HLParsingTools.ReadOneOfAny(
                                                               tokens,
                                                               funcIdx - 1,
                                                               new[] { HLTokenType.OpWord, HLTokenType.OpTypeVoid }
                                                              );

                        int modStart = funcIdx - 1 - 1;

                        IHlToken[] mods = HLParsingTools.ReadNoneOrManyOf(
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
                        tokens.RemoveRange(start, end - start + 1);

                        tokens.Insert(
                                      start,
                                      new FunctionDefinitionToken(
                                                                  funcName,
                                                                  typeName,
                                                                  ParseArgumentList(args.Reverse().ToList()),
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

        public void ParseImports(List<IHlToken> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == HLTokenType.OpNumSign && tokens.Count > i + 2)
                {
                    if (tokens[i + 1].ToString() == "import" && tokens[i + 2].Type == HLTokenType.OpStringLiteral)
                    {
                        m_ImportedItems.Add(tokens[i + 2].ToString());
                        tokens.RemoveRange(i, 3);
                    }
                }
            }
        }

        public void ParseIncludes(List<IHlToken> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == HLTokenType.OpNumSign && tokens.Count > i + 2)
                {
                    if (tokens[i + 1].ToString() == "include" && tokens[i + 2].Type == HLTokenType.OpStringLiteral)
                    {
                        string c = UriResolver.GetFilePath(m_Directory, tokens[i + 2].ToString());
                        m_IncludedFiles.Add(c ?? tokens[i + 2].ToString());
                        tokens.RemoveRange(i, 3);
                    }
                }
            }
        }

        public void ParseOneLineStrings(List<IHlToken> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (i < tokens.Count - 1 &&
                     tokens[i].Type == HLTokenType.OpDoubleQuote)
                {
                    int idx = tokens.FindIndex(i + 1, t => t.Type == HLTokenType.OpNewLine);
                    int endQuote = tokens.FindIndex(i + 1, t => t.Type == HLTokenType.OpDoubleQuote);

                    if (idx == -1)
                    {
                        idx = tokens.Count - 1;
                    }

                    if (endQuote == -1 || endQuote > idx)
                    {
                        EventManager<ErrorEvent>.SendEvent(
                                                              new HLTokenReadEvent(
                                                                   HLTokenType.OpDoubleQuote,
                                                                   HLTokenType.OpNewLine
                                                                  )
                                                             );

                        return;
                    }

                    string ConcatContent()
                    {
                        List<IHlToken> content = tokens.GetRange(i + 1, endQuote - i - 1);

                        return OriginalText.Substring(
                                                      content.First().SourceIndex,
                                                      tokens[i + 1 + content.Count].SourceIndex -
                                                      content.First().SourceIndex
                                                     );
                    }

                    IHlToken newToken = new HLTextToken(
                                                        HLTokenType.OpStringLiteral,
                                                        ConcatContent(),
                                                        tokens[i].SourceIndex
                                                       );

                    tokens.RemoveRange(i, endQuote - i + 1);
                    tokens.Insert(i, newToken);
                }
            }
        }

        public void ParseVarDefToken(List<IHlToken> tokens, HLParserSettings settings)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == HLTokenType.OpWord || settings.MemberModifiers.ContainsValue(tokens[i].Type))
                {
                    List<IHlToken> line = HLParsingTools.ReadUntilAny(
                                                                         tokens,
                                                                         i,
                                                                         1,
                                                                         new[]
                                                                         {
                                                                             HLTokenType.OpSemicolon,
                                                                             HLTokenType.Eof,
                                                                             HLTokenType.OpBlockBracketOpen,
                                                                             HLTokenType.OpBlockBracketClose
                                                                         }
                                                                        ).
                                                            ToList();

                    IHlToken[] mods =
                        HLParsingTools.ReadNoneOrManyOf(line, 0, 1, settings.MemberModifiers.Values.ToArray());

                    if (!HLParsingTools.ReadOneOrNone(line, mods.Length, HLTokenType.OpWord, out IHlToken type) ||
                         !HLParsingTools.ReadOneOrNone(line, mods.Length + 1, HLTokenType.OpWord, out IHlToken name))
                    {
                        i += line.Count;

                        continue;
                    }

                    IHlToken num = null;

                    if (line.Count == mods.Length + 2)
                    {
                        tokens.RemoveRange(i, line.Count + 1);

                        if (mods.All(x => x.Type != HLTokenType.OpPublicMod) &&
                             mods.All(x => x.Type != HLTokenType.OpPrivateMod))
                        {
                            mods = mods.Append(new HLTextToken(HLTokenType.OpPrivateMod, "private", 0)).ToArray();
                        }

                        tokens.Insert(i, new VariableDefinitionToken(name, type, mods, line.ToArray(), null, null));

                        continue;
                    }

                    if (line[mods.Length + 2].Type == HLTokenType.OpIndexerBracketOpen)
                    {
                        num = HLParsingTools.ReadAny(line, mods.Length + 3);
                        HLParsingTools.ReadOne(line, mods.Length + 4, HLTokenType.OpIndexerBracketClose);
                        tokens.RemoveRange(i, line.Count + 1);

                        if (mods.All(x => x.Type != HLTokenType.OpPublicMod) &&
                             mods.All(x => x.Type != HLTokenType.OpPrivateMod))
                        {
                            mods = mods.Append(new HLTextToken(HLTokenType.OpPrivateMod, "private", 0)).ToArray();
                        }

                        tokens.Insert(i, new VariableDefinitionToken(name, type, mods, line.ToArray(), null, num));

                        continue;
                    }

                    if (line[mods.Length + 2].Type == HLTokenType.OpEquality)
                    {
                        tokens.RemoveRange(i, line.Count + 1);
                        IHlToken[] init = line.Skip(mods.Length + 3).ToArray();

                        if (mods.All(x => x.Type != HLTokenType.OpPublicMod) &&
                             mods.All(x => x.Type != HLTokenType.OpPrivateMod))
                        {
                            mods = mods.Append(new HLTextToken(HLTokenType.OpPrivateMod, "private", 0)).ToArray();
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

        internal string GetTempVar(uint initValue)
        {
            VariableData tmp = GetFreeTempVar(initValue);
            m_UsedTempVars.Add(tmp.GetName());

            return tmp.GetName();
        }

        internal string GetTempVarCopy(string initValue)
        {
            VariableData tmp = GetFreeTempVarCopy(initValue);
            m_UsedTempVars.Add(tmp.GetName());

            return tmp.GetName();
        }

        internal string GetTempVarDref(string initValue)
        {
            VariableData tmp = GetFreeTempVarDref(initValue);
            m_UsedTempVars.Add(tmp.GetName());

            return tmp.GetName();
        }

        internal string GetTempVarLoad(string initValue)
        {
            VariableData tmp = GetFreeTempVarLoad(initValue);
            m_UsedTempVars.Add(tmp.GetName());

            return tmp.GetName();
        }

        internal string GetTempVarPop()
        {
            VariableData tmp = GetFreeTempVarPop();
            m_UsedTempVars.Add(tmp.GetName());

            return tmp.GetName();
        }

        internal string Parse(HLExpression[] block, bool printHead = true, string appendAfterProg = "HLT")
        {
            foreach (HLExpression hlExpression in block)
            {
                ExpressionTarget c = Parse(hlExpression, new ExpressionTarget());
                ReleaseTempVar(c.ResultAddress);
            }

            StringBuilder sb = new StringBuilder();
            StringBuilder funcCode = new StringBuilder();
            GenerateFunctionCode(funcCode, printHead);

            if (printHead)
            {
                sb.AppendLine("; ________________ Includes ________________");
            }

            foreach (string includedFile in m_IncludedFiles)
            {
                sb.AppendLine($":include \"{includedFile}\"");
            }

            if (printHead)
            {
                sb.AppendLine("; ________________ CONST VALUES ________________");
            }

            foreach (KeyValuePair<string, string> keyValuePair in ConstValTypes)
            {
                if (m_Parent == null)
                {
                    sb.AppendLine($":const {keyValuePair.Key} {keyValuePair.Value} linker:hide");
                }
                else if (!m_Parent.ConstValTypes.ContainsKey(keyValuePair.Key))
                {
                    m_Parent.ConstValTypes[keyValuePair.Key] = keyValuePair.Value;
                }
            }

            if (printHead)

            {
                sb.AppendLine("; ________________ VARIABLE FIELDS ________________");
            }

            foreach (KeyValuePair<string, VariableData> keyValuePair in m_VariableMap)
            {
                if (m_Parent == null)
                {
                    if (keyValuePair.Value.InitContent != null)
                    {
                        sb.AppendLine(
                                      $":data {keyValuePair.Value.GetFinalName()} \"{keyValuePair.Value.InitContent}\" linker:hide"
                                     );
                    }
                    else
                    {
                        sb.AppendLine(
                                      $":data {keyValuePair.Value.GetFinalName()} {keyValuePair.Value.Size} {(keyValuePair.Value.IsVisible ? "" : "linker:hide")}"
                                     );
                    }
                }
                else if (!m_Parent.m_VariableMap.ContainsKey(keyValuePair.Key))
                {
                    m_Parent.m_VariableMap[keyValuePair.Key] = keyValuePair.Value;
                }
            }

            if (printHead)
            {
                sb.AppendLine("; ________________ MAIN PROGRAM CODE ________________");
            }

            string[] code = EmitterResult.Get();

            foreach (string programCode in code)
            {
                sb.AppendLine(programCode);
            }

            if (!string.IsNullOrEmpty(appendAfterProg))
            {
                sb.AppendLine(appendAfterProg);
            }

            sb.Append(funcCode);

            return m_ParsedText = sb.ToString();
        }

        internal ExpressionTarget Parse(HLExpression expr, ExpressionTarget outputTarget = default)
        {
            Type t = expr.GetType();

            if (TypeMap.ContainsCompiler(t))
            {
                return TypeMap.Get(t).Parse(this, expr, outputTarget);
            }

            EventManager<ErrorEvent>.SendEvent(new ExpressionCompilerNotFoundEvent(expr));

            return new ExpressionTarget();
        }

        internal void ReleaseTempVar(string varName)
        {
            if (m_Settings.OptimizeTempVarUsage && m_UsedTempVars.Contains(varName))
            {
                m_UsedTempVars.Remove(varName);
                m_UnusedTempVars.Enqueue(varName);
            }
        }

        #endregion

        #region Private

        private static IHlToken[] ParseArgumentList(List<IHlToken> tokens)
        {
            HLExpressionReader reader = new HLExpressionReader(tokens);

            IHlToken current = reader.GetNext();
            List<VariableDefinitionToken> ret = new List<VariableDefinitionToken>();

            while (current.Type != HLTokenType.Eof)
            {
                IHlToken typeName = current;
                Eat(HLTokenType.OpWord);
                IHlToken varName = current;
                Eat(HLTokenType.OpWord);

                ret.Add(
                        new VariableDefinitionToken(
                                                    varName,
                                                    typeName,
                                                    new IHlToken[0],
                                                    new[] { typeName, varName },
                                                    null
                                                   )
                       );

                if (current.Type == HLTokenType.Eof)
                {
                    return ret.ToArray();
                }

                Eat(HLTokenType.OpComma);
            }

            return ret.Cast<IHlToken>().ToArray();

            void Eat(HLTokenType type)
            {
                if (current.Type != type)
                {
                    EventManager<ErrorEvent>.SendEvent(new HLTokenReadEvent(type, current.Type));
                }

                current = reader.GetNext();
            }
        }

        private void GenerateFunctionCode(StringBuilder sb, bool printHead)
        {
            if (printHead)
            {
                sb.AppendLine("; ________________ FUNCTION CODE ________________");
            }

            foreach (KeyValuePair<string, FunctionData> keyValuePair in FunctionMap)
            {
                if (keyValuePair.Value.GetCompiledOutput().Length == 0)
                {
                    continue;
                }

                if (m_Parent == null)
                {
                    sb.AppendLine("." + keyValuePair.Key + (keyValuePair.Value.Public ? "" : " linker:hide"));
                }

                foreach (string s in keyValuePair.Value.GetCompiledOutput())
                {
                    if (m_Parent == null)
                    {
                        sb.AppendLine(s);
                    }
                    else if (!m_Parent.FunctionMap.ContainsKey(keyValuePair.Key))
                    {
                        m_Parent.FunctionMap[keyValuePair.Key] = keyValuePair.Value;
                    }
                }
            }
        }

        private VariableData GetFreeTempVar(uint initValue)
        {
            if (m_UnusedTempVars.Count != 0)
            {
                string oldName = m_UnusedTempVars.Dequeue();

                EmitterResult.Emit("LOAD", oldName, initValue.ToString());

                return m_VariableMap[oldName];
            }

            string name = GetUniqueName("tmp");

            if (initValue != 0)
            {
                EmitterResult.Emit($"LOAD", name, initValue.ToString());
            }

            return m_VariableMap[name] = new VariableData(name, name, 1, TypeSystem.GetOrAdd("var"), false);
        }

        private VariableData GetFreeTempVarCopy(string initValue)
        {
            if (m_UnusedTempVars.Count != 0)
            {
                string oldName = m_UnusedTempVars.Dequeue();
                EmitterResult.Emit($"COPY", initValue, oldName);

                return m_VariableMap[oldName];
            }

            string name = GetUniqueName("tmp");

            EmitterResult.Emit($"COPY", initValue, name);

            return m_VariableMap[name] = new VariableData(name, name, 1, TypeSystem.GetOrAdd("var"), false);
        }

        private VariableData GetFreeTempVarDref(string initValue)
        {
            if (m_UnusedTempVars.Count != 0)
            {
                string oldName = m_UnusedTempVars.Dequeue();
                EmitterResult.Emit($"DREF", initValue, oldName);

                return m_VariableMap[oldName];
            }

            string name = GetUniqueName("tmp");

            EmitterResult.Emit($"DREF", initValue, name);

            return m_VariableMap[name] = new VariableData(name, name, 1, TypeSystem.GetOrAdd("var"), false);
        }

        private VariableData GetFreeTempVarLoad(string initValue)
        {
            if (m_UnusedTempVars.Count != 0)
            {
                string oldName = m_UnusedTempVars.Dequeue();
                EmitterResult.Emit($"LOAD", oldName, initValue);

                return m_VariableMap[oldName];
            }

            string name = GetUniqueName("tmp");

            EmitterResult.Emit($"LOAD", name, initValue);

            return m_VariableMap[name] = new VariableData(name, name, 1, TypeSystem.GetOrAdd("var"), false);
        }

        private VariableData GetFreeTempVarPop()
        {
            if (m_UnusedTempVars.Count != 0)
            {
                string oldName = m_UnusedTempVars.Dequeue();
                EmitterResult.Emit("POP", oldName);

                return m_VariableMap[oldName];
            }

            string name = GetUniqueName("tmp");

            EmitterResult.Emit("POP", name);

            return m_VariableMap[name] = new VariableData(name, name, 1, TypeSystem.GetOrAdd("var"), false);
        }

        private string GetPrefix()
        {
            if (m_Parent == null)
            {
                return m_ScopeId + "_";
            }

            return m_Parent.GetPrefix() + m_ScopeId + "_";
        }

        private void ParseDependencies()
        {
            ExpressionParser exP = new ExpressionParser();

            for (int i = 0; i < m_IncludedFiles.Count; i++)
            {
                string includedFile = m_IncludedFiles[i];

                if (m_Parent != null && m_Parent.m_IncludedFiles.Contains(includedFile))
                {
                    continue;
                }

                if (includedFile.EndsWith(".vhl"))
                {
                    Log($"Importing File: {includedFile}");

                    string name = Path.GetFullPath(
                        includedFile.StartsWith(m_Directory)
                            ? includedFile.Remove(includedFile.Length - 4, 4)
                            : m_Directory +
                              "/" +
                              includedFile.Remove(includedFile.Length - 4, 4)
                    );

                    Uri import = new Uri(includedFile, UriKind.RelativeOrAbsolute);
                    Uri dir = new Uri(Directory.GetCurrentDirectory()+"/", UriKind.Absolute);
                    if (import.IsAbsoluteUri)
                    {
                    	Log($"Relative Base Path: {dir.OriginalString}");
                        name = dir.MakeRelativeUri(import).OriginalString;
                        name = name.Remove(name.Length - 4, 4);
                    	Log($"Fixed Path to File: {includedFile} => {name}");
                    }



                    string newInclude = m_DataStore.GetStorePath("HL2VASM", name);
                    string file = Path.GetFullPath(name + ".vhl");

                    HLCompilation comp = exP.Parse(
                                                   File.ReadAllText(file),
                                                   Path.GetDirectoryName(file),
                                                   m_DataStore
                                                  );

                    File.WriteAllText(newInclude, comp.Parse());
                    ExternalSymbols.AddRange(comp.FunctionMap.Values.Where(x => x.Public));
                    ExternalSymbols.AddRange(comp.ExternalSymbols);
                    includedFile = newInclude;
                }

                OnCompiledIncludedScript?.Invoke(
                                                 Path.GetFullPath(m_Directory + "/" + m_IncludedFiles[i]),
                                                 includedFile
                                                );

                m_IncludedFiles[i] = includedFile;
            }
        }

        private void ParseReservedKeys(List<IHlToken> tokens, HLParserSettings settings)
        {
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                IHlToken token = tokens[i];

                if (token.Type == HLTokenType.OpWord && settings.ReservedKeys.ContainsKey(token.ToString()))
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
            List<IHlToken> block)
        {
            HLExpressionParser p = HLExpressionParser.Create(new HLExpressionReader(block));
            HLExpression[] exprs = p.Parse();

            foreach (HLExpression hlToken in exprs)
            {
                if (hlToken is HLVarDefOperand t)
                {
                    HLTypeDefinition tt = ts.GetOrAdd(t.VariableDefinition.TypeName.ToString());

                    if (t.VariableDefinition.Size != null)
                    {
                        tt = new ArrayTypeDefintion(
                                                    tt,
                                                    t.VariableDefinition.Size.ToString().ParseUInt()
                                                   );
                    }

                    HLPropertyDefinition pdef = new HLPropertyDefinition(
                                                                         t.VariableDefinition.Name.ToString(),
                                                                         tt,
                                                                         t.VariableDefinition.Modifiers
                                                                        );

                    tdef.AddMember(pdef);
                }
                else
                {
                    EventManager<ErrorEvent>.SendEvent(
                                                          new HLTokenReadEvent(
                                                                               HLTokenType.OpVariableDefinition,
                                                                               hlToken.Type
                                                                              )
                                                         );
                }
            }
        }

        private void ParseTypeDefinitions(HLTypeSystem ts, HLParserSettings settings, List<IHlToken> tokens)
        {
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                if (tokens[i].Type == HLTokenType.OpBlockToken ||
                     tokens[i].Type == HLTokenType.OpNamespaceDefinition)
                {
                    ParseTypeDefinitions(ts, settings, tokens[i].GetChildren());
                }

                if (tokens[i].Type == HLTokenType.OpClass)
                {
                    IHlToken name = HLParsingTools.ReadOne(tokens, i + 1, HLTokenType.OpWord);

                    IHlToken[] mods = HLParsingTools.ReadNoneOrManyOf(
                                                                      tokens,
                                                                      i - 1,
                                                                      -1,
                                                                      settings.ClassModifiers.Values.ToArray()
                                                                     ).
                                                     Reverse().
                                                     ToArray();

                    IHlToken baseClass = null;
                    int offset = 2;

                    if (HLParsingTools.ReadOneOrNone(
                                                      tokens,
                                                      i + offset,
                                                      HLTokenType.OpColon,
                                                      out IHlToken inhColon
                                                     ))
                    {
                        baseClass = HLParsingTools.ReadOne(tokens, i + offset + 1, HLTokenType.OpWord);
                        Log("Found base class: {0}", baseClass);
                        offset += 2;
                    }

                    IHlToken block = HLParsingTools.ReadOne(
                                                            tokens,
                                                            i + offset,
                                                            HLTokenType.OpBlockToken
                                                           );

                    int start = i - mods.Length;
                    int end = i + offset + 1;
                    tokens.RemoveRange(start, end - start);
                    HLTypeDefinition def = ts.CreateEmptyType(name.ToString());
                    ParseTypeDefinitionBody(ts, def, block.GetChildren());
                    i = start;
                }
            }
        }

        private void ProcessImports()
        {
            for (int i = 0; i < m_ImportedItems.Count; i++)
            {
                IImporter imp = ImporterSystem.Get(m_ImportedItems[i]);
                bool error = true;

                if (imp is IFileImporter fimp)
                {
                    error = false;
                    m_IncludedFiles.Add(fimp.ProcessImport(m_ImportedItems[i]));
                }

                if (imp is IDataImporter dimp)
                {
                    error = false;
                    IExternalData[] data = dimp.ProcessImport(m_ImportedItems[i]);

                    ExternalSymbols.AddRange(data);
                }

                if (imp == null)
                {
                    EventManager<ErrorEvent>.SendEvent(new ImporterNotFoundEvent(m_ImportedItems[i]));
                }

                if (error)
                {
                    EventManager<ErrorEvent>.SendEvent(new ImporterTypeInvalidEvent(imp.GetType()));
                }
            }
        }

        private void RemoveComments(List<IHlToken> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (i < tokens.Count - 1 &&
                     tokens[i].Type == HLTokenType.OpFwdSlash &&
                     tokens[i + 1].Type == HLTokenType.OpFwdSlash)
                {
                    int idx = tokens.FindIndex(i + 2, t => t.Type == HLTokenType.OpNewLine);

                    if (idx == -1)
                    {
                        idx = tokens.Count - 1;
                    }

                    tokens.RemoveRange(i, idx - i);
                }
            }
        }

        #endregion

    }

}
