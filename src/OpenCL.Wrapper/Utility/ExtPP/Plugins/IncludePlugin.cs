using System.Collections.Generic;
using System.IO;
using System.Linq;

using Utility.ADL;
using Utility.ExtPP.Base;
using Utility.ExtPP.Base.Interfaces;
using Utility.ExtPP.Base.Plugins;
using Utility.ExtPP.Base.settings;

using Utils = Utility.ExtPP.Base.Utils;

namespace Utility.ExtPP.Plugins
{
    public class IncludePlugin : AbstractFullScriptPlugin
    {

        public override string[] Cleanup => new[] { IncludeKeyword };

        public override ProcessStage ProcessStages => ProcessStage.OnMain;

        public override string[] Prefix => new[] { "inc", "Include" };

        public string IncludeKeyword { get; set; } = "#include";

        public string IncludeInlineKeyword { get; set; } = "#includeinl";

        public string Separator { get; set; } = " ";

        public override List<CommandInfo> Info { get; } = new List<CommandInfo>
                                                          {
                                                              new CommandInfo(
                                                                              "set-include",
                                                                              "i",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       IncludePlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       IncludeKeyword
                                                                                   )
                                                                                  ),
                                                                              "Sets the keyword that is used to include other files into the build process."
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-include-inline",
                                                                              "ii",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       IncludePlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       IncludeInlineKeyword
                                                                                   )
                                                                                  ),
                                                                              "Sets the keyword that is used to insert other files directly into the current file"
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-separator",
                                                                              "s",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       IncludePlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       Separator
                                                                                   )
                                                                                  ),
                                                                              "Sets the separator that is used to separate the include statement from the filepath"
                                                                             )
                                                          };

        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defTable)
        {
            settings.ApplySettings(Info, this);
        }


        public override bool FullScriptStage(ISourceScript script, ISourceManager sourceManager, IDefinitions defs)
        {
            Logger.Log(LogType.Log, "Disovering Include Statments...", PLUGIN_MIN_SEVERITY);
            List<string> source = script.GetSource().ToList();
            string currentPath = Path.GetDirectoryName(script.GetFileInterface().GetFilePath());

            //bool hasIncludedInline;
            //do
            //{
            //    hasIncludedInline = false;
            //    for (int i = source.Count - 1; i >= 0; i--)
            //    {
            //        if (Utils.IsStatement(source[i], IncludeInlineKeyword))
            //        {
            //            Logger.Log(LogType.Log, "Found Inline Include Statement...", PLUGIN_MIN_SEVERITY + 1);
            //            string[] args = Utils.SplitAndRemoveFirst(source[i], Separator);
            //            if (args.Length == 0)
            //            {
            //                Logger.Log(LogType.Error, "No File Specified", 1);
            //                continue;
            //            }

            //            if (Utils.FileExistsRelativeTo(currentPath, args[0]))
            //            {
            //                Logger.Log(LogType.Log, "Replacing Inline Keyword with file content",
            //                    PLUGIN_MIN_SEVERITY + 2);
            //                source.RemoveAt(i);

            //                source.InsertRange(i, IOManager.ReadAllLines(Path.Combine(currentPath, args[0])));
            //                hasIncludedInline = true;
            //            }
            //            else
            //            {
            //                Logger.Log(LogType.Error, $"File does not exist: {args[0]}", 1);
            //            }
            //        }
            //    }

            //    script.SetSource(source.ToArray());
            //} while (hasIncludedInline);


            string[] inlIncs = Utils.FindStatements(source.ToArray(), IncludeInlineKeyword);

            foreach (string inlInc in inlIncs)
            {
                Logger.Log(LogType.Log, $"Processing Statement: {inlInc}", PLUGIN_MIN_SEVERITY + 1);
                bool tmp = GetISourceScript(sourceManager, inlInc, currentPath, true, out List<ISourceScript> sources);

                if (tmp)
                {
                    foreach (ISourceScript sourceScript in sources)
                    {
                        Logger.Log(
                                   LogType.Log,
                                   $"Processing Inline Include: {Path.GetFileName(sourceScript.GetFileInterface().GetKey())}",
                                   PLUGIN_MIN_SEVERITY + 2
                                  );

                        if (!sourceManager.IsIncluded(sourceScript))
                        {
                            sourceManager.AddToTodo(sourceScript);
                        }
                        else
                        {
                            sourceManager.FixOrder(sourceScript);
                        }
                    }
                }
                else
                {
                    return
                        false; //We crash if we didnt find the file. but if the user forgets to specify the path we will just log the error
                }
            }

            string[] incs = Utils.FindStatements(source.ToArray(), IncludeKeyword)
                                 .Where(x => !Utils.IsStatement(x, IncludeInlineKeyword)).ToArray();

            foreach (string includes in incs)
            {
                Logger.Log(LogType.Log, $"Processing Statement: {includes}", PLUGIN_MIN_SEVERITY + 1);
                bool tmp = GetISourceScript(
                                            sourceManager,
                                            includes,
                                            currentPath,
                                            false,
                                            out List<ISourceScript> sources
                                           );
                if (tmp)
                {
                    foreach (ISourceScript sourceScript in sources)
                    {
                        Logger.Log(
                                   LogType.Log,
                                   $"Processing Include: {Path.GetFileName(sourceScript.GetFileInterface().GetKey())}",
                                   PLUGIN_MIN_SEVERITY + 2
                                  );

                        if (!sourceManager.IsIncluded(sourceScript))
                        {
                            sourceManager.AddToTodo(sourceScript);
                        }
                        else
                        {
                            sourceManager.FixOrder(sourceScript);
                        }
                    }
                }
                else
                {
                    return
                        false; //We crash if we didnt find the file. but if the user forgets to specify the path we will just log the error
                }
            }

            Logger.Log(LogType.Log, "Inclusion of Files Finished", PLUGIN_MIN_SEVERITY);
            return true;
        }


        private bool GetISourceScript(
            ISourceManager manager, string statement, string currentPath, bool isInline,
            out List<ISourceScript> scripts)
        {
            string[] vars = Utils.SplitAndRemoveFirst(statement, Separator);

            scripts = new List<ISourceScript>();
            if (vars.Length != 0)
            {
                ImportResult importInfo = manager.GetComputingScheme()(vars, currentPath);
                if (!importInfo)
                {
                    Logger.Log(LogType.Error, "Invalid Include Statement", 1);
                    return false;
                }

                string filepath = importInfo.GetString("filename");
                importInfo.RemoveEntry("filename");
                string key = importInfo.GetString("key");
                importInfo.RemoveEntry("key");
                string originalDefinedName = importInfo.GetString("definedname");
                importInfo.RemoveEntry("definedname");


                IFileContent cont = new FilePathContent(filepath, originalDefinedName);
                cont.SetKey(key);
                if (manager.TryCreateScript(out ISourceScript iss, Separator, cont, importInfo, isInline))
                {
                    scripts.Add(iss);
                }


                for (int index = scripts.Count - 1; index >= 0; index--)
                {
                    ISourceScript sourceScript = scripts[index];
                    if (sourceScript.GetFileInterface().HasValidFilepath &&
                        !Utils.FileExistsRelativeTo(currentPath, sourceScript.GetFileInterface()))
                    {
                        Logger.Log(LogType.Error, $"Could not find File: {sourceScript.GetFileInterface()}", 1);
                        scripts.RemoveAt(index);
                    }
                }


                return true;
            }


            return false;
        }

    }
}