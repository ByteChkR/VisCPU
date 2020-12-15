using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using VisCPU.HL.Compiler;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.Parser.Tokens.Combined;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.Logging;

namespace VisCPU.HL
{
    public class HLVariableNotFoundEvent : ErrorEvent
    {

        private const string EVENT_KEY = "hl-var-not-found";
        public HLVariableNotFoundEvent(string varName, bool canContinue) : base($"Can not find variable: {varName}", EVENT_KEY, canContinue)
        {
        }

    }

    public class HLCompilation : VisBase
    {

        protected override LoggerSystems SubSystem => LoggerSystems.HL_Compiler;

        internal const string CONST_VAL_TYPE = "const_var";
        internal const string VAL_TYPE = "var";

        private static uint counter;

        public static void ResetCounter() => counter = 0;
        internal readonly Dictionary<string, string> ConstValTypes = new Dictionary<string, string>();
        internal readonly List<IExternalData> ExternalSymbols = new List<IExternalData>();
        internal readonly Dictionary<string, FunctionData> FunctionMap = new Dictionary<string, FunctionData>();

        private readonly List<string> IncludedFiles = new List<string>();

        internal readonly string OriginalText;
        private readonly HLCompilation parent;
        private string scopeID = "_";
        internal readonly List<string> ProgramCode = new List<string>();
        private readonly Dictionary<string, VariableData> VariableMap = new Dictionary<string, VariableData>();
        private readonly string Directory;
        private string ParsedText;

        internal Dictionary<Type, IHLExpressionCompiler> TypeMap = new Dictionary<Type, IHLExpressionCompiler>
                                                                   {
                                                                       {
                                                                           typeof(HLVarDefOperand),
                                                                           new VariableDefinitionExpressionCompiler()
                                                                       },
                                                                       {
                                                                           typeof(HLArrayAccessorOp),
                                                                           new ArrayAccessCompiler()
                                                                       },
                                                                       {
                                                                           typeof(HLVarOperand),
                                                                           new VarExpressionCompiler()
                                                                       },
                                                                       {
                                                                           typeof(HLValueOperand),
                                                                           new ConstExpressionCompiler()
                                                                       },
                                                                       {
                                                                           typeof(HLInvocationOp),
                                                                           new InvocationExpressionCompiler()
                                                                       },
                                                                       {
                                                                           typeof(HLFuncDefOperand),
                                                                           new FunctionDefinitionCompiler()
                                                                       },
                                                                       { typeof(HLIfOp), new IfBlockCompiler() },
                                                                       {
                                                                           typeof(HLReturnOp),
                                                                           new ReturnExpressionCompiler()
                                                                       },
                                                                       {
                                                                           typeof(HLWhileOp),
                                                                           new WhileExpressionCompiler()
                                                                       },
                                                                       {
                                                                           typeof(HLUnaryOp),
                                                                           new OperatorCompilerCollection<HLUnaryOp>(
                                                                                new Dictionary<HLTokenType,
                                                                                    HLExpressionCompiler<HLUnaryOp>>
                                                                                {
                                                                                    {
                                                                                        HLTokenType.OpBang,
                                                                                        new BoolNotExpressionCompiler()
                                                                                    }
                                                                                }
                                                                               )
                                                                       },
                                                                       {
                                                                           typeof(HLBinaryOp),
                                                                           new OperatorCompilerCollection<HLBinaryOp>(
                                                                                new Dictionary<HLTokenType,
                                                                                    HLExpressionCompiler<HLBinaryOp>>
                                                                                {
                                                                                    {
                                                                                        HLTokenType.OpEquality,
                                                                                        new EqualExpressionCompiler()
                                                                                    },
                                                                                    {
                                                                                        HLTokenType.OpPlus,
                                                                                        new AddExpressionCompiler()
                                                                                    },
                                                                                    {
                                                                                        HLTokenType.OpMinus,
                                                                                        new SubtractExpressionCompiler()
                                                                                    },
                                                                                    {
                                                                                        HLTokenType.OpAsterisk,
                                                                                        new MultiplyExpressionCompiler()
                                                                                    },
                                                                                    {
                                                                                        HLTokenType
                                                                                            .OpComparison,
                                                                                        new EqualityComparisonCompiler()
                                                                                    },
                                                                                    {
                                                                                        HLTokenType
                                                                                            .OpLogicalOr,
                                                                                        new BoolOrExpressionCompiler()
                                                                                    },
                                                                                    {
                                                                                        HLTokenType
                                                                                            .OpLogicalAnd,
                                                                                        new BoolAndExpressionCompiler()
                                                                                    },
                                                                                    {
                                                                                        HLTokenType
                                                                                            .OpPipe,
                                                                                        new BitwiseOrExpressionCompiler()
                                                                                    },
                                                                                    {
                                                                                        HLTokenType
                                                                                            .OpAnd,
                                                                                        new BitwiseAndExpressionCompiler()
                                                                                    },
                                                                                    {
                                                                                        HLTokenType
                                                                                            .OpPercent,
                                                                                        new ModuloExpressionCompiler()
                                                                                    },
                                                                                    {
                                                                                        HLTokenType
                                                                                            .OpFwdSlash,
                                                                                        new DivideExpressionCompiler()
                                                                                    },
                                                                                    {
                                                                                        HLTokenType
                                                                                            .OpCap,
                                                                                        new BitwiseXOrExpressionCompiler()
                                                                                    },
                                                                                    {
                                                                                        HLTokenType
                                                                                            .OpLessThan,
                                                                                        new LessComparisonCompiler()
                                                                                    },
                                                                                    {
                                                                                        HLTokenType
                                                                                            .OpGreaterThan,
                                                                                        new GreaterComparisonCompiler()
                                                                                    },
                                                                                    {
                                                                                        HLTokenType
                                                                                            .OpLessOrEqual,
                                                                                        new
                                                                                            LessEqualComparisonCompiler()
                                                                                    },
                                                                                    {
                                                                                        HLTokenType
                                                                                            .OpGreaterOrEqual,
                                                                                        new
                                                                                            GreaterEqualComparisonCompiler()
                                                                                    },
                                                                                    {
                                                                                        HLTokenType
                                                                                            .OpFunctionDefinition,
                                                                                        new MultiplyExpressionCompiler()
                                                                                    }
                                                                                }
                                                                               )
                                                                       }
                                                                   };


        public HLCompilation(string originalText, string directory)
        {
            OriginalText = originalText;
            Directory = directory;
        }

        public HLCompilation(HLCompilation parent, string id)
        {
            scopeID = id;
            OriginalText = parent.OriginalText;
            Directory = parent.Directory;
            VariableMap = new Dictionary<string, VariableData>(parent.VariableMap);
            ConstValTypes = new Dictionary<string, string>(parent.ConstValTypes);
            FunctionMap = new Dictionary<string, FunctionData>(parent.FunctionMap);
            ExternalSymbols = new List<IExternalData>(parent.ExternalSymbols);
            IncludedFiles = new List<string>(parent.IncludedFiles);

            this.parent = parent;
        }

        private string GetPrefix()
        {
            if (parent == null) return scopeID + "_";

            return parent.GetPrefix() + scopeID + "_";
        }

        public string GetFinalName(string name)
        {
            return GetPrefix() + name;
        }

        public bool ContainsVariable(string var)
        {
            return VariableMap.ContainsKey(GetFinalName(var)) || parent != null && parent.ContainsVariable(var);
        }
        public bool ContainsLocalVariable(string var)
        {
            return VariableMap.ContainsKey(GetFinalName(var));
        }

        public void CreateVariable(string name, uint size)
        {
            VariableMap[GetFinalName(name)] = new VariableData(name, GetFinalName(name), size);
        }

        public void CreateVariable(string name, string content)
        {
            VariableMap[GetFinalName(name)] = new VariableData(name, GetFinalName(name), content);
        }

        public VariableData GetVariable(string name)
        {
            if (VariableMap.ContainsKey(GetFinalName(name))) return VariableMap[GetFinalName(name)];


            if (parent == null)
            {
                EventManager<ErrorEvent>.SendEvent(new HLVariableNotFoundEvent(name, false));

                return new VariableData();
            }

            return parent.GetVariable(name);
        }

        public event Action<string, string> OnCompiledIncludedScript;

        private void RemoveComments(List<IHLToken> tokens)
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


        public void ParseBlocks(List<IHLToken> tokens)
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

                    List<IHLToken> content = tokens.GetRange(start + 1, i - start - 1).ToList();
                    tokens.RemoveRange(start, i - start + 1);
                    ParseBlocks(content);
                    tokens.Insert(start, new BlockToken(content.ToArray(), start));
                    i = start;
                }
            }
        }

        private void ParseReservedKeys(List<IHLToken> tokens, HLParserSettings settings)
        {
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                IHLToken token = tokens[i];
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

        public void ParseFunctionToken(List<IHLToken> tokens, HLParserSettings settings)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == HLTokenType.OpBlockToken)
                {
                    if (!HLParsingTools.ReadOneOrNone(tokens, i - 1, HLTokenType.OpBracketClose, out IHLToken bClose))
                    {
                        continue;
                    }


                    List<IHLToken> argPart = new List<IHLToken> { bClose };
                    IHLToken[] args = HLParsingTools.ReadUntil(tokens, i - 2, -1, HLTokenType.OpBracketOpen);
                    argPart.AddRange(args);
                    IHLToken argOpenBracket = HLParsingTools.ReadOne(
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

                    IHLToken funcName = HLParsingTools.ReadOne(tokens, funcIdx, HLTokenType.OpWord);


                    IHLToken typeName = null;
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
                        IHLToken[] mods = HLParsingTools.ReadNoneOrManyOf(
                                                                          tokens,
                                                                          modStart,
                                                                          -1,
                                                                          settings.MemberModifiers.Values
                                                                              .ToArray()
                                                                         ).Reverse().ToArray();
                        int start = modStart - mods.Length + 1;
                        int end = i;
                        IHLToken block = tokens[i];
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

        private static IHLToken[] ParseArgumentList(List<IHLToken> tokens)
        {
            HLExpressionReader reader = new HLExpressionReader(tokens);

            IHLToken current = reader.GetNext();
            List<VariableDefinitionToken> ret = new List<VariableDefinitionToken>();

            while (current.Type != HLTokenType.EOF)
            {
                IHLToken typeName = current;
                Eat(HLTokenType.OpWord);
                IHLToken varName = current;
                Eat(HLTokenType.OpWord);
                ret.Add(
                        new VariableDefinitionToken(
                                                    varName,
                                                    typeName,
                                                    new IHLToken[0],
                                                    new[] { typeName, varName },
                                                    null
                                                   )
                       );
                if (current.Type == HLTokenType.EOF)
                {
                    return ret.ToArray();
                }

                Eat(HLTokenType.OpComma);
            }

            return ret.ToArray();


            void Eat(HLTokenType type)
            {
                if (current.Type != type)
                {
                    EventManager<ErrorEvent>.SendEvent(new HLTokenInvalidReadEvent(type, current.Type));
                }

                current = reader.GetNext();
            }
        }

        public void ParseIncludes(List<IHLToken> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == HLTokenType.OpNumSign && tokens.Count > i + 2)
                {
                    if (tokens[i + 1].ToString() == "include" && tokens[i + 2].Type == HLTokenType.OpStringLiteral)
                    {
                        string c = UriResolver.GetFilePath(Directory, tokens[i + 2].ToString());
                        UriResolver.Resolve(Directory, tokens[i + 2].ToString());
                        IncludedFiles.Add(c ?? tokens[i + 2].ToString());
                        tokens.RemoveRange(i, 3);
                    }
                }
            }
        }

        public void ParseOneLineStrings(List<IHLToken> tokens)
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
                                                              new HLTokenInvalidReadEvent(
                                                                   HLTokenType.OpDoubleQuote,
                                                                   HLTokenType.OpNewLine
                                                                  )
                                                             );

                        return;
                    }

                    List<IHLToken> content = tokens.GetRange(i + 1, endQuote - i - 1);

                    string ConcatContent()
                    {
                        StringBuilder sb = new StringBuilder();
                        bool lastWasWord = false;
                        foreach (IHLToken token in content)
                        {
                            if (lastWasWord && token.Type == HLTokenType.OpWord) sb.Append(" ");
                            sb.Append(token);
                            lastWasWord = token.Type == HLTokenType.OpWord;
                        }

                        return sb.ToString();
                    }

                    IHLToken newToken = new HLTextToken(
                                                        HLTokenType.OpStringLiteral,
                                                        ConcatContent(),
                                                        tokens[i].SourceIndex
                                                       );
                    tokens.RemoveRange(i, endQuote - i + 1);
                    tokens.Insert(i, newToken);
                }
            }
        }

        internal string Parse(HLExpression[] block, bool printHead = true, string appendAfterProg = "HLT")
        {
            foreach (HLExpression hlExpression in block)
            {
                Parse(hlExpression);
            }

            StringBuilder sb = new StringBuilder();
            StringBuilder funcCode = new StringBuilder();
            GenerateFunctionCode(funcCode, printHead);
            if (printHead)
            {
                sb.AppendLine("; ________________ Includes ________________");
            }

            foreach (string includedFile in IncludedFiles)
            {
                sb.AppendLine($":include \"{includedFile}\"");
            }

            if (printHead)
            {
                sb.AppendLine("; ________________ CONST VALUES ________________");
            }

            foreach (KeyValuePair<string, string> keyValuePair in ConstValTypes)
            {
                if (parent == null)
                {
                    sb.AppendLine($":const {keyValuePair.Key} {keyValuePair.Value} linker:hide");
                }
                else if (!parent.ConstValTypes.ContainsKey(keyValuePair.Key))
                {
                    parent.ConstValTypes[keyValuePair.Key] = keyValuePair.Value;
                }
            }

            if (printHead)

            {
                sb.AppendLine("; ________________ VARIABLE FIELDS ________________");
            }

            foreach (KeyValuePair<string, VariableData> keyValuePair in VariableMap)
            {
                if (parent == null)
                {
                    if (keyValuePair.Value.InitContent != null)
                    {
                        sb.AppendLine($":data {keyValuePair.Value.GetFinalName()} \"{keyValuePair.Value.InitContent}\" linker:hide");
                    }
                    else
                        sb.AppendLine($":data {keyValuePair.Value.GetFinalName()} {keyValuePair.Value.Size} linker:hide");
                }
                else if (!parent.VariableMap.ContainsKey(keyValuePair.Key))
                {
                    parent.VariableMap[keyValuePair.Key] = keyValuePair.Value;
                }
            }


            if (printHead)
            {
                sb.AppendLine("; ________________ MAIN PROGRAM CODE ________________");
            }

            foreach (string programCode in ProgramCode)
            {
                sb.AppendLine(programCode);
            }

            if (!string.IsNullOrEmpty(appendAfterProg))
            {
                sb.AppendLine(appendAfterProg);
            }

            sb.Append(funcCode);

            return ParsedText = sb.ToString();
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

                if (parent == null)
                {
                    sb.AppendLine("." + keyValuePair.Key + (keyValuePair.Value.Public ? "" : " linker:hide"));
                }

                foreach (string s in keyValuePair.Value.GetCompiledOutput())
                {
                    if (parent == null)
                    {
                        sb.AppendLine(s);
                    }
                    else if (!parent.FunctionMap.ContainsKey(keyValuePair.Key))
                    {
                        parent.FunctionMap[keyValuePair.Key] = keyValuePair.Value;
                    }
                }
            }
        }

        public string Parse(bool printHead = true, string appendAfterProg = "HLT")
        {
            if (ParsedText != null)
            {
                return ParsedText;
            }

            HLParserSettings hlpS = new HLParserSettings();
            HLParserBaseReader br = new HLParserBaseReader(hlpS, OriginalText);

            List<IHLToken> tokens = br.ReadToEnd();
            ParseOneLineStrings(tokens);
            RemoveComments(tokens);
            ParseIncludes(tokens);
            ParseReservedKeys(tokens, hlpS);
            tokens = tokens.Where(x => x.Type != HLTokenType.OpNewLine).ToList();
            ParseBlocks(tokens);

            ParseFunctionToken(tokens, hlpS);

            HLExpressionParser p = HLExpressionParser.Create(new HLExpressionReader(tokens));

            ParseDependencies();

            return Parse(p.Parse(), printHead, appendAfterProg);
        }

        private void ParseDependencies()
        {
            ExpressionParser exP = new ExpressionParser();
            for (int i = 0; i < IncludedFiles.Count; i++)
            {
                string includedFile = IncludedFiles[i];
                if (includedFile.EndsWith(".vhl"))
                {
                    Log($"Importing File: {includedFile}");

                    string name =Path.GetFullPath( includedFile.StartsWith( Directory )
                                      ? includedFile.Remove( includedFile.Length - 3, 3 )
                                      : Directory +
                                        "/" +
                                        includedFile.Remove( includedFile.Length - 3, 3 ));

                    string newInclude = Path.GetFullPath( name + ".vasm" );
                    string file = Path.GetFullPath( name + ".vhl" );
                    HLCompilation comp = exP.Parse(File.ReadAllText(file), Path.GetDirectoryName(file));
                    File.WriteAllText(newInclude, comp.Parse());
                    ExternalSymbols.AddRange(comp.FunctionMap.Values.Where(x => x.Public));
                    ExternalSymbols.AddRange(comp.ExternalSymbols);
                    includedFile = newInclude;
                }

                OnCompiledIncludedScript?.Invoke(Path.GetFullPath(Directory + "/" + IncludedFiles[i]), includedFile);
                IncludedFiles[i] = includedFile;
            }
        }

        internal ExpressionTarget Parse(HLExpression expr, ExpressionTarget outputTarget = default)
        {
            Type t = expr.GetType();
            if (TypeMap.ContainsKey(t))
            {
                return TypeMap[t].Parse(this, expr, outputTarget);
            }

            EventManager<ErrorEvent>.SendEvent(new ExpressionCompilerNotFoundEvent(expr));

            return new ExpressionTarget();
        }

        internal string GetUniqueName(string prefix = null)
        {
            return (prefix == null ? "" : prefix + "_") + counter++;
        }

        internal string GetTempVar(string prefix = "TMP")
        {
            string name = GetUniqueName(prefix);

            VariableMap[name] = new VariableData(name, name, 1);
            return name;
        }

    }

    public class HLTokenInvalidReadEvent : ErrorEvent
    {

        private const string EVENT_KEY = "hl-parser-token-invalid";
        public HLTokenInvalidReadEvent(HLTokenType expected, HLTokenType got) : base($"Expected Token '{expected}' but got '{got}'", EVENT_KEY, false)
        {
        }

    }

    public class ExpressionCompilerNotFoundEvent : ErrorEvent
    {

        private const string EVENT_KEY = "hl-compiler-not-found";

        public ExpressionCompilerNotFoundEvent(HLExpression expr) : base(
                                                                           $"No Compiler found for expression: ({expr.Type}) '{expr}'",
                                                                           EVENT_KEY,
                                                                           false
                                                                          )
        {
        }

    }
}