using System.Collections.Generic;

using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.Modules
{

    public class ModuleSubSystem : ConsoleSystem
    {

        public override Dictionary < string, ConsoleSubsystem > SubSystems =>
            new Dictionary < string, ConsoleSubsystem >
            {
                { "clean", new ModuleCleanSubSystem() },
                { "create", new ModuleCreateSubSystem() },
                { "pack", new ModulePackSubSystem() },
                { "restore", new ModuleRestoreSubSystem() },
                { "publish", new ModulePublishLocalSubSystem() },
                { "add", new ModuleAddDependencySubSystem() },
                { "list", new ListLocalPackagesSubSystem() }
            };

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

    }

}
