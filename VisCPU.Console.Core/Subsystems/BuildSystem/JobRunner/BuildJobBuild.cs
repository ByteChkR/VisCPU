using VisCPU.ProjectSystem.BuildSystem;
using VisCPU.ProjectSystem.Data;
using System.IO;

namespace VisCPU.Console.Core.Subsystems.BuildSystem.JobRunner
{

    public class BuildJobBuild : BuildJobRunner
    {
        public override string RunnerName => "build";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            ProjectBuildJob job )
        {
            string old = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(projectRoot);
            ProgramBuilder.Build( job.Arguments );
            Directory.SetCurrentDirectory(old);
        }

        #endregion
    }

}
