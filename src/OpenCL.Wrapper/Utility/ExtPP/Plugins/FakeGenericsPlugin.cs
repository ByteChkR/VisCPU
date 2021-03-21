using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Utility.ADL;
using Utility.ExtPP.Base;
using Utility.ExtPP.Base.Interfaces;
using Utility.ExtPP.Base.Plugins;
using Utility.ExtPP.Base.settings;
using Utility.FastString;

using Utils = Utility.ExtPP.Base.Utils;

namespace Utility.ExtPP.Plugins
{
    public class FakeGenericsPlugin : AbstractFullScriptPlugin
    {

        public override string[] Prefix => new[] { "gen", "FakeGen" };

        public override ProcessStage ProcessStages =>
            Stage.ToLower(CultureInfo.InvariantCulture) == "onload"
                ? ProcessStage.OnLoadStage
                : ProcessStage.OnMain;

        public string Stage { get; set; } = "onmain";

        public string GenericKeyword { get; set; } = "#type";

        public string Separator { get; set; } = " ";

        public override List<CommandInfo> Info { get; } = new List<CommandInfo>
                                                          {
                                                              new CommandInfo(
                                                                              "set-genkeyword",
                                                                              "g",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       FakeGenericsPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       GenericKeyword
                                                                                   )
                                                                                  ),
                                                                              "Sets the keyword that is used when writing pseudo generic code."
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-separator",
                                                                              "s",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       FakeGenericsPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       Separator
                                                                                   )
                                                                                  ),
                                                                              "Sets the separator that is used to separate different generic types"
                                                                             ),
                                                              new CommandInfo(
                                                                              "set-stage",
                                                                              "ss",
                                                                              PropertyHelper.GetPropertyInfo(
                                                                                   typeof(
                                                                                       FakeGenericsPlugin
                                                                                   ),
                                                                                   nameof(
                                                                                       Stage)
                                                                                  ),
                                                                              "Sets the Stage Type of the Plugin to be Executed OnLoad or OnFinishUp"
                                                                             )
                                                          };

        public override void Initialize(Settings settings, ISourceManager sourceManager, IDefinitions defs)
        {
            settings.ApplySettings(Info, this);
            sourceManager.SetComputingScheme(ComputeNameAndKey_Generic);
        }


        private ImportResult ComputeNameAndKey_Generic(string[] vars, string currentPath)
        {
            ImportResult ret = new ImportResult();

            string filePath = "";

            if (!Utils.TryResolvePathIncludeParameter(vars))
            {
                return ret;
            }

            string[] genParams = vars.Length > 1 ? vars.SubArray(1, vars.Length - 1).ToArray() : new string[0];

            string rel = Path.Combine(currentPath, vars[0]);
            string key = Path.GetFullPath(rel);

            filePath = rel;
            key += genParams.Length > 0 ? "." + genParams.Unpack(Separator) : "";
            if (genParams.Length != 0)
            {
                ret.SetValue("genParams", genParams);
            }

            ret.SetValue("definedname", vars[0]);
            ret.SetValue("filename", filePath);
            ret.SetValue("key", key);
            ret.SetResult(true);
            return ret;
        }


        public override bool FullScriptStage(ISourceScript file, ISourceManager sourceManager, IDefinitions defs)
        {
            if (!file.HasValueOfType<string[]>("genParams"))
            {
                return true; //No error, we just dont have any generic parameters to replace.
            }

            string[] genParams = file.GetValueFromCache<string[]>("genParams");

            Logger.Log(LogType.Log, "Discovering Generic Keywords...", PLUGIN_MIN_SEVERITY);
            if (genParams != null && genParams.Length > 0)
            {
                for (int i = genParams.Length - 1; i >= 0; i--)
                {
                    Logger.Log(
                               LogType.Log,
                               $"Replacing Keyword {GenericKeyword}{i} with {genParams[i]} in file {file.GetKey()}",
                               PLUGIN_MIN_SEVERITY + 1
                              );
                    Utils.ReplaceKeyWord(
                                         file.GetSource(),
                                         genParams[i],
                                         GenericKeyword + i
                                        );
                }
            }


            Logger.Log(LogType.Log, "Generic Keyword Replacement Finished", PLUGIN_MIN_SEVERITY);

            return true;
        }

    }
}