using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.Console.Core.Subsystems.Origins;
using VisCPU.HL.Modules;
using VisCPU.Utility.ArgumentParser;

namespace VisCPU.Console.Core.Subsystems
{
    
    
    public class ModuleSubSystem:ConsoleSubsystem
    {
        private readonly Dictionary<string, ConsoleSubsystem> subsystems =
            new Dictionary<string, ConsoleSubsystem>
            {
                { "create", new ModuleCreateSubSystem() },
                { "pack", new ModulePackSubSystem() },
                { "restore", new ModuleRestoreSubSystem() },
                { "publish", new ModulePublishLocalSubSystem() },
                { "add", new ModuleAddDependencySubSystem() }
            };

        public override void Run( IEnumerable < string > args )
        {
            CLISettings s = CLISettings.Create();
            ArgumentSyntaxParser.Parse( args.ToArray(), s );
            VisConsole.RunConsole(s, args.ToArray(), subsystems);
        }

    }

    public class ModuleAddDependencySubSystem : ConsoleSubsystem
    {

        public override void Run(IEnumerable<string> args)
        {
            ModuleTarget t =
                ModuleManager.LoadModuleTarget( Path.Combine( Directory.GetCurrentDirectory(), "project.json" ) );

            t.Dependencies.Add(
                               new ModuleDependency
                               {
                                   ModuleName = "ModuleDependency",
                                   ModuleVersion = "ANY"
                               }
                              );

            ModuleManager.SaveModuleTarget( t, Path.Combine( Directory.GetCurrentDirectory(), "project.json" ) );
        }

    }

}