using VisCPU.ProjectSystem.Data;

namespace VisCPU.ProjectSystem.BuildSystem
{

    public abstract class BuildJobRunner
    {

        public abstract string RunnerName { get; }

        #region Public

        public abstract void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            ProjectBuildJob job );

        #endregion

    }

}
