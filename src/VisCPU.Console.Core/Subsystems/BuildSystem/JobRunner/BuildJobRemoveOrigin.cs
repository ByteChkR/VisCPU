using VisCPU.Console.Core.Subsystems.Origins;
using VisCPU.Utility.ProjectSystem.BuildSystem;
using VisCPU.Utility.ProjectSystem.Data;

namespace VisCPU.Console.Core.Subsystems.BuildSystem.JobRunner
{

    public class BuildJobRemoveOrigin : BuildJobRunner
    {
        public override string RunnerName => "remove-origins";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            ProjectBuildJob job )
        {
            foreach ( string argumentsKey in job.Arguments.Keys )
            {
                RemoveOriginSubSystem.RemoveOrigin( argumentsKey );
            }
        }

        #endregion
    }

}
