using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Utility.ADL;
using Utility.ExtPP.Base;
using Utility.ExtPP.Base.Interfaces;
using Utility.ExtPP.Base.Plugins;
using Utility.ExtPP.Base.settings;
using Utility.FastString;

using Utils = Utility.ExtPP.Base.Utils;

namespace Utility.ExtPP.Plugins
{
    public class ConditionalPlugin : AbstractFullScriptPlugin
    {

        private static readonly StringBuilder Sb = new StringBuilder();

        public override string[] Cleanup => new[] { DefineKeyword, UndefineKeyword };

        public override string[] Prefix => new[] { "con", "Conditional" };

        public override ProcessStage ProcessStages =>
            Stage.ToLower(CultureInfo.InvariantCulture) == "onload"
                ? ProcessStage.OnLoadStage
                : ProcessStage.OnMain;

        public string StartCondition { get; set; } = "#if";

        public string ElseIfCondition { get; set; } = "#elseif";

        public string ElseCondition { get; set; } = "#else";

        public string EndCondition { get; set; } = "#endif";

        public string UndefineKeyword { get; set; } = "#undefine";

        public string DefineKeyword { get; set; } = "#define";

        public string OrOperator { get; set; } = "||";

        public string NotOperator { get; set; } = "!";

        public string AndOperator { get; set; } = "&&";

        public string Separator { get; set; } = " ";

        public bool EnableDefine { get; set; } = true;

        public bool EnableUndefine { get; set; } = true;

        public string Stage { get; set; } = "onload";


        public override List<CommandInfo> Info { get; } = new List<CommandInfo>
                                                          {
                                                              new CommandInfo(
                                                                              "set-define",
                                                                              "d",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ConditionalPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       DefineKeyword
                                                                                   )
                                                                                  ),
                                                                              "Sets the keyword that is used to define variables during the compilation."
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-undefine",
                                                                              "u",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ConditionalPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       UndefineKeyword
                                                                                   )
                                                                                  ),
                                                                              "Sets the keyword that is used to undefine previously defined variables during the compilation."
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-if",
                                                                              "if",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ConditionalPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       StartCondition
                                                                                   )
                                                                                  ),
                                                                              "Sets the keyword that is used to start a new condition block."
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-elseif",
                                                                              "elif",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ConditionalPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       ElseIfCondition
                                                                                   )
                                                                                  ),
                                                                              "Sets the keyword that is used to continue a previously started condition block with another condition block."
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-else",
                                                                              "else",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ConditionalPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       ElseCondition
                                                                                   )
                                                                                  ),
                                                                              "Sets the keyword that is used to start a new condition block that is taken when the previous blocks evaluated to false."
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-endif",
                                                                              "eif",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ConditionalPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       EndCondition
                                                                                   )
                                                                                  ),
                                                                              "Sets the keyword that is used to end a previously started condition block."
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-not",
                                                                              "n",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ConditionalPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       NotOperator
                                                                                   )
                                                                                  ),
                                                                              "Sets the keyword that is used to negate an expression in if conditions."
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-and",
                                                                              "a",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ConditionalPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       AndOperator
                                                                                   )
                                                                                  ),
                                                                              "Sets the keyword for the logical AND operator"
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-or",
                                                                              "o",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ConditionalPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       OrOperator
                                                                                   )
                                                                                  ),
                                                                              "Sets the keyword for the logical OR operator"
                                                                             ),
                                                              new CommandInfo(
                                                                              "enable-define",
                                                                              "eD",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ConditionalPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       EnableDefine
                                                                                   )
                                                                                  ),
                                                                              "Enables/Disables the detection of define statements(defines can still be set via the defines object/the command line)"
                                                                             ),
                                                              new CommandInfo(
                                                                              "enable-undefine",
                                                                              "eU",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ConditionalPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       EnableUndefine
                                                                                   )
                                                                                  ),
                                                                              "Enables/Disables the detection of undefine statements"
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-stage",
                                                                              "ss",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ConditionalPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       Stage)
                                                                                  ),
                                                                              "Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp"
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-separator",
                                                                              "s",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       ConditionalPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       Separator
                                                                                   )
                                                                                  ),
                                                                              "Sets the separator that is used to separate different generic types"
                                                                             )
                                                          };


        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defs)
        {
            settings.ApplySettings(Info, this);
        }


        public override bool FullScriptStage(ISourceScript file, ISourceManager todo, IDefinitions defs)
        {
            Logger.Log(
                       LogType.Log,
                       $"Starting Condition Solver passes on file: {Path.GetFileName(file.GetFileInterface().GetKey())}",
                       PLUGIN_MIN_SEVERITY
                      );
            bool ret = true;
            int openIf = 0;
            bool foundConditions = false;
            bool elseIsValid = false;
            bool expectEndOrIf = false;
            List<string> lastPass = file.GetSource().ToList();
            List<string> solvedFile = new List<string>();
            int passCount = 0;
            do
            {
                passCount++;
                Logger.Log(LogType.Log, $"Starting Condition Solver pass: {passCount}", PLUGIN_MIN_SEVERITY + 1);

                foundConditions = false;
                elseIsValid = false;
                for (int i = 0; i < lastPass.Count; i++)
                {
                    string line = lastPass[i].TrimStart();
                    if (IsKeyWord(line, StartCondition))
                    {
                        Logger.Log(LogType.Log, $"Found a {StartCondition} Statement", PLUGIN_MIN_SEVERITY + 2);
                        KeyValuePair<bool, int> prep = PrepareForConditionalEvaluation(
                             line,
                             defs,
                             lastPass,
                             i,
                             solvedFile
                            );

                        elseIsValid = prep.Key;
                        i += prep.Value;

                        openIf++;
                        foundConditions = true;
                        expectEndOrIf = false;
                    }
                    else if (IsKeyWord(line, ElseIfCondition))
                    {
                        Logger.Log(LogType.Log, $"Found a {ElseIfCondition} Statement", PLUGIN_MIN_SEVERITY + 2);
                        if (!expectEndOrIf && openIf > 0)
                        {
                            KeyValuePair<bool, int> prep = PrepareForConditionalEvaluation(
                                 line,
                                 defs,
                                 lastPass,
                                 i,
                                 solvedFile,
                                 !elseIsValid
                                );

                            elseIsValid &= prep.Key;
                            i += prep.Value;
                            foundConditions = true;
                        }
                        else if (expectEndOrIf)
                        {
                            Logger.Log(
                                       LogType.Error,
                                       $"A {ElseCondition} can not be followed by an {ElseIfCondition}",
                                       1
                                      );
                            ret = false;
                            break;
                        }
                        else
                        {
                            Logger.Log(
                                       LogType.Error,
                                       $"A {ElseIfCondition} should be preceeded by a {StartCondition}",
                                       1
                                      );
                            ret = false;
                            break;
                        }
                    }
                    else if (IsKeyWord(line, ElseCondition))
                    {
                        if (openIf > 0)
                        {
                            Logger.Log(LogType.Log, $"Found a {ElseCondition} Statement", PLUGIN_MIN_SEVERITY + 2);
                            int size = GetBlockSize(lastPass, i);
                            if (elseIsValid)
                            {
                                solvedFile.AddRange(lastPass.SubArray(i + 1, size));
                                Logger.Log(LogType.Log, "Adding Branch To Solved File.", PLUGIN_MIN_SEVERITY + 3);
                            }
                            else
                            {
                                Logger.Log(
                                           LogType.Log,
                                           "Ignored since a previous condition was true",
                                           PLUGIN_MIN_SEVERITY + 3
                                          );
                            }

                            i += size;
                            foundConditions = true;
                            expectEndOrIf = true;
                        }
                        else
                        {
                            Logger.Log(
                                       LogType.Error,
                                       $"A {ElseCondition} should be preceeded by a {StartCondition}",
                                       1
                                      );
                            ret = false;
                            break;
                        }
                    }
                    else if (IsKeyWord(line, EndCondition))
                    {
                        if (openIf > 0)
                        {
                            expectEndOrIf = false;
                            openIf--;
                        }
                        else
                        {
                            ret = false;

                            Logger.Log(LogType.Error, $"A {EndCondition} should be preceeded by a {StartCondition}", 1);
                            break;
                        }
                    }
                    else if (EnableDefine &&
                             line.StartsWith(DefineKeyword))
                    {
                        Logger.Log(LogType.Log, $"Found a {DefineKeyword} Statement", PLUGIN_MIN_SEVERITY + 2);
                        defs.Set(Utils.SplitAndRemoveFirst(line, Separator));
                        solvedFile.Add(lastPass[i]);
                    }
                    else if (EnableUndefine &&
                             line.StartsWith(UndefineKeyword))
                    {
                        Logger.Log(LogType.Log, $"Found a {UndefineKeyword} Statement", PLUGIN_MIN_SEVERITY + 2);
                        defs.Unset(Utils.SplitAndRemoveFirst(line, Separator));
                        solvedFile.Add(lastPass[i]);
                    }
                    else
                    {
                        solvedFile.Add(lastPass[i]);
                    }
                }

                if (openIf != 0)
                {
                    throw new InvalidOperationException(
                                                        $"Can not find an appropriate closing tag for {openIf} {StartCondition} statements."
                                                       );
                }

                if (ret)
                {
                    lastPass = solvedFile;
                }
                else
                {
                    break;
                }

                solvedFile = new List<string>();
            } while (foundConditions);

            file.SetSource(lastPass.ToArray());


            Logger.Log(LogType.Log, "Conditional Solver Finished", PLUGIN_MIN_SEVERITY);

            return ret;
        }


        private KeyValuePair<bool, int> PrepareForConditionalEvaluation(
            string line, IDefinitions defs,
            IReadOnlyList<string> lastPass, int i, List<string> solvedFile, bool skipAdd = false)
        {
            bool r = EvaluateConditional(line, defs);
            Logger.Log(LogType.Log, $"Evaluation: {r}", PLUGIN_MIN_SEVERITY + 4);
            bool elseIsValid = !r;
            int size = GetBlockSize(lastPass, i);
            if (r && !skipAdd)
            {
                solvedFile.AddRange(lastPass.SubArray(i + 1, size));
                Logger.Log(LogType.Log, "Adding Branch To Solved File.", PLUGIN_MIN_SEVERITY + 3);
            }

            return new KeyValuePair<bool, int>(elseIsValid, size);
        }

        private int GetBlockSize(IReadOnlyList<string> source, int start)
        {
            Logger.Log(LogType.Log, "Finding End of conditional block...", PLUGIN_MIN_SEVERITY + 5);
            int tolerance = 0;
            for (int i = start + 1; i < source.Count; i++)
            {
                string line = source[i].Trim();
                if (line.StartsWith(StartCondition))
                {
                    Logger.Log(LogType.Log, "Found nested opening conditional block...", PLUGIN_MIN_SEVERITY + 6);
                    i += GetBlockSize(source, i); //Skip Indices that are "inside" the if clause
                    tolerance++;
                }

                else if (line.StartsWith(EndCondition) ||
                         line.StartsWith(ElseIfCondition) ||
                         line.StartsWith(ElseCondition))
                {
                    if (tolerance == 0)
                    {
                        Logger.Log(LogType.Log, "Found correct ending conditional block...", PLUGIN_MIN_SEVERITY + 5);
                        return i - start - 1;
                    }

                    if (line.StartsWith(EndCondition))
                    {
                        Logger.Log(LogType.Log, "Found an ending conditional block...", PLUGIN_MIN_SEVERITY + 6);
                        tolerance--;
                    }
                }
            }

            throw new InvalidOperationException(
                                                "Can not determine Ending tag for Keyword:" +
                                                source[start] +
                                                " at line " +
                                                start
                                               );
        }

        private bool EvaluateConditional(string expression, IDefinitions defs)
        {
            string condition = FixCondition(Utils.SplitAndRemoveFirst(expression, Separator).Unpack(Separator));

            string[] cs = condition.Pack(Separator).ToArray();
            return EvaluateConditional(cs, defs);
        }

        private bool EvaluateConditional(string[] expression, IDefinitions defs)
        {
            Logger.Log(LogType.Log, $"Evaluating Expression: {expression.Unpack(" ")}", PLUGIN_MIN_SEVERITY + 4);

            bool ret = true;
            bool isOr = false;
            bool expectOperator = false;

            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i] == OrOperator || expression[i] == AndOperator)
                {
                    isOr = expression[i] == OrOperator;
                    expectOperator = false;
                }
                else if (expression[i] == "(")
                {
                    if (expectOperator)
                    {
                        isOr = false;
                    }

                    expectOperator = true;

                    int size = IndexOfClosingBracket(expression, i) - i - 1;
                    bool tmp = EvaluateConditional(expression.SubArray(i + 1, size).ToArray(), defs);
                    if (isOr)
                    {
                        ret |= tmp;
                    }
                    else
                    {
                        ret &= tmp;
                    }

                    i += size;
                }
                else
                {
                    if (expectOperator)
                    {
                        isOr = false;
                    }

                    expectOperator = true;
                    bool tmp = EvaluateExpression(expression[i], defs);
                    if (isOr)
                    {
                        ret |= tmp;
                    }
                    else
                    {
                        ret &= tmp;
                    }
                }
            }

            return ret;
        }

        private bool EvaluateExpression(string expression, IDefinitions defs)
        {
            Logger.Log(LogType.Log, $"Evaluating Expression: {expression}", PLUGIN_MIN_SEVERITY + 6);
            bool neg = expression.StartsWith(NotOperator);
            if (expression == NotOperator)
            {
                Logger.Log(LogType.Error, "Single not Operator found. Will break the compilation.", 1);
                return false;
            }

            string exp = expression;
            if (neg)
            {
                exp = expression.Substring(1, expression.Length - 1);
            }

            bool val = defs.Check(exp);

            val = neg ? !val : val;

            return val;
        }

        private string FixCondition(string line)
        {
            Logger.Log(LogType.Log, $"Fixing expression: {line}", PLUGIN_MIN_SEVERITY + 7);


            string r = line;
            r = SurroundWithSpaces(r, OrOperator);
            r = SurroundWithSpaces(r, AndOperator);
            r = SurroundWithSpaces(r, "(");
            r = SurroundWithSpaces(r, ")");
            string rr = Utils.RemoveExcessSpaces(r, Separator);

            Logger.Log(LogType.Log, $"Fixed condition(new): {rr}", PLUGIN_MIN_SEVERITY + 7);
            return rr;
        }

        private int IndexOfClosingBracket(string[] expression, int openBracketIndex)
        {
            Logger.Log(LogType.Log, "Finding Closing Bracket...", PLUGIN_MIN_SEVERITY + 7);
            int tolerance = 0;
            for (int i = openBracketIndex + 1; i < expression.Length; i++)
            {
                if (expression[i] == "(")
                {
                    Logger.Log(
                               LogType.Log,
                               "Found Nested opening Bracket, adjusting tolerance.",
                               PLUGIN_MIN_SEVERITY + 8
                              );
                    tolerance++;
                }
                else if (expression[i] == ")")
                {
                    if (tolerance == 0)
                    {
                        Logger.Log(LogType.Log, "Found Correct Closing Bracket", 7);
                        return i;
                    }

                    Logger.Log(
                               LogType.Log,
                               "Found Nested Closing Bracket, adjusting tolerance.",
                               PLUGIN_MIN_SEVERITY + 8
                              );
                    tolerance--;
                }
            }

            return -1;
        }


        private string SurroundWithSpaces(string line, string keyword)
        {
            Sb.Clear();
            Sb.Append(line);
            Logger.Log(LogType.Log, $"Surrounding {keyword} with spaces...", PLUGIN_MIN_SEVERITY + 9);
            Sb.Replace(keyword, " " + keyword + " ");
            return Sb.ToString();
        }


        private static bool IsKeyWord(string line, string keyword)
        {
            string tmp = line.TrimStart();

            return tmp.StartsWith(keyword);
        }

    }
}