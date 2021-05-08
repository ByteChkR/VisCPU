using VisCPU.Console.Core.Subsystems.Project;
using VisCPU.Utility.ProjectSystem.BuildSystem;
using VisCPU.Utility.ProjectSystem.Data;

namespace VisCPU.Console.Core.Subsystems.BuildSystem.JobRunner
{

    public class BuildJobClean : BuildJobRunner
    {
        public override string RunnerName => "clean";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            ProjectBuildJob job )
        {
            ProjectCleanSubSystem.Clean( projectRoot );
        }

        #endregion
    }

}
