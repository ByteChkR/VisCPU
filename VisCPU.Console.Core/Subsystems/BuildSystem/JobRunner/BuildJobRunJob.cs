using VisCPU.ProjectSystem.BuildSystem;
using VisCPU.ProjectSystem.Data;
using System.IO;

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
            string old = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(projectRoot);
            ProgramRunner.Run( job.Arguments );
            Directory.SetCurrentDirectory(old);
        }

        #endregion
    }

}
