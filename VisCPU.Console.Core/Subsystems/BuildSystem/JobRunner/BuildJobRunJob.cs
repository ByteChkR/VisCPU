using VisCPU.ProjectSystem.BuildSystem;
using VisCPU.ProjectSystem.Data;

namespace VisCPU.Console.Core.Subsystems.BuildSystem.JobRunner
{

    public class BuildJobRunJob : BuildJobRunner
    {
        public override string RunnerName => "run";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            ProjectBuildJob job )
        {
            ProgramRunner.Run( job.Arguments );
        }

        #endregion
    }

}
