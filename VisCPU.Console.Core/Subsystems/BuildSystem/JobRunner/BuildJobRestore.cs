using VisCPU.Console.Core.Subsystems.Project;
using VisCPU.HL.Modules.BuildSystem;
using VisCPU.HL.Modules.Data;

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
            BuildJob job )
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
