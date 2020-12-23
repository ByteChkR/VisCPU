using System.Collections.Generic;
using System.Linq;
using VisCPU.Console.Core.Settings;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.Utility.ArgumentParser;

namespace VisCPU.Console.Core.Subsystems.Modules
{
    public class ListLocalPackagesSubSystem : ConsoleSubsystem
    {
        public override void Run(IEnumerable<string> args)
        {
            OriginSettings s = OriginSettings.Create();

            foreach (KeyValuePair<string, string> keyValuePair in s.origins)
            {
                LocalModuleManager lm = new LocalModuleManager(keyValuePair.Value);
                Log($"{keyValuePair.Key} : {keyValuePair.Value}");
                foreach (ModulePackage modulePackage in lm.GetPackages())
                {
                    Log($"\t{modulePackage.ModuleName}");
                    foreach (string modulePackageModuleVersion in modulePackage.ModuleVersions)
                    {
                        Log($"\t\t{modulePackageModuleVersion}");
                    }
                }

            }
        }
    }

    public class ModuleSubSystem : ConsoleSubsystem
    {
        private readonly Dictionary<string, ConsoleSubsystem> subsystems =
            new Dictionary<string, ConsoleSubsystem>
            {
                {"create", new ModuleCreateSubSystem()},
                {"pack", new ModulePackSubSystem()},
                {"restore", new ModuleRestoreSubSystem()},
                {"publish", new ModulePublishLocalSubSystem()},
                {"add", new ModuleAddDependencySubSystem()},
                {"list", new ListLocalPackagesSubSystem()}
            };

        public override void Run(IEnumerable<string> args)
        {
            CLISettings s = CLISettings.Create();
            OriginSettings os = OriginSettings.Create();
            ArgumentSyntaxParser.Parse(args.ToArray(), s);
            VisConsole.RunConsole(s, args.ToArray(), subsystems);
        }
    }
}