using VisCPU.Console.Core.Subsystems.Project;
using VisCPU.Utility.ProjectSystem.BuildSystem;
using VisCPU.Utility.ProjectSystem.Data;

namespace VisCPU.Console.Core.Subsystems.BuildSystem.JobRunner
{

    public class BuildJobRestore : BuildJobRunner
    {
        public override string RunnerName => "restore";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            ProjectBuildJob job )
        {
            if ( !job.Arguments.TryGetValue( "origin", out string repo ) )
            {
                repo = "local";
            }

            ProjectRestoreSubSystem.Restore( projectRoot, repo );
        }

        #endregion
    }

}
