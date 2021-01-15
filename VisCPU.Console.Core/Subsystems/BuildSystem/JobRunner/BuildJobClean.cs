using VisCPU.Console.Core.Subsystems.Project;
using VisCPU.HL.Modules.BuildSystem;
using VisCPU.HL.Modules.Data;

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
            BuildJob job )
        {
            ProjectCleanSubSystem.Clean( projectRoot );
        }

        #endregion

    }

}
