using VisCPU.Console.Core.Subsystems.Origins;
using VisCPU.HL.Modules.BuildSystem;
using VisCPU.HL.Modules.Data;

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
            BuildJob job )
        {
            foreach ( string argumentsKey in job.Arguments.Keys )
            {
                RemoveOriginSubSystem.RemoveOrigin( argumentsKey );
            }
        }

        #endregion

    }

}
