using System.Collections.Generic;

using VisCPU.Console.Core.Subsystems.BuildSystem;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.Project
{

    public class ProjectSubSystem : ConsoleSystem
    {

        public override Dictionary < string, ConsoleSubsystem > SubSystems =>
            new Dictionary < string, ConsoleSubsystem >
            {
                { "clean", new ProjectCleanSubSystem() },
                { "new", new ProjectCreateSubSystem() },
                { "restore", new ProjectRestoreSubSystem() },
                { "publish", new ProjectPublishSubSystem() },
                { "add", new ProjectAddDependencySubSystem() },
                { "make", new BuildJobSystem() }
            };

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

    }

}
