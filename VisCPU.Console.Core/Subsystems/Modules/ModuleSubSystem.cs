using System.Collections.Generic;

using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.Modules
{
    public class ModuleSubSystem : ConsoleSystem
    {
        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;
        public override Dictionary<string, ConsoleSubsystem> SubSystems =>
            new Dictionary<string, ConsoleSubsystem>
            {
                {"clean", new ModuleCleanSubSystem()},
                {"create", new ModuleCreateSubSystem()},
                {"pack", new ModulePackSubSystem()},
                {"restore", new ModuleRestoreSubSystem()},
                {"publish", new ModulePublishLocalSubSystem()},
                {"add", new ModuleAddDependencySubSystem()},
                {"update", new ModuleUpdateLocalSubSystem()},
                {"list", new ListLocalPackagesSubSystem()}
            };
    }
}