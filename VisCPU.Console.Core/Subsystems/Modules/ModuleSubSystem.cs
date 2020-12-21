using System.Collections.Generic;
using System.Linq;
using VisCPU.Console.Core.Settings;
using VisCPU.Utility.ArgumentParser;

namespace VisCPU.Console.Core.Subsystems.Modules
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
}