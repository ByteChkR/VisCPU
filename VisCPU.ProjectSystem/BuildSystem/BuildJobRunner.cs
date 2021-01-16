using VisCPU.HL.Modules.Data;

namespace VisCPU.HL.Modules.BuildSystem
{

    public abstract class BuildJobRunner
    {

        public abstract string RunnerName { get; }

        #region Public

        public abstract void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job );

        #endregion

    }

}
