using VisCPU.HL.Modules.BuildSystem;
using VisCPU.HL.Modules.Data;

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
            BuildJob job )
        {
            ProgramRunner.Run( job.Arguments );
        }

        #endregion

    }

}
