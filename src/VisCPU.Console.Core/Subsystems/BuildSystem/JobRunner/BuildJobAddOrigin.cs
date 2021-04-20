using System.Collections.Generic;

using VisCPU.Console.Core.Subsystems.Origins;
using VisCPU.Utility.ProjectSystem.BuildSystem;
using VisCPU.Utility.ProjectSystem.Data;

namespace VisCPU.Console.Core.Subsystems.BuildSystem.JobRunner
{

    public class BuildJobAddOrigin : BuildJobRunner
    {

        public override string RunnerName => "add-origin";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            ProjectBuildJob job )
        {
            foreach ( KeyValuePair < string, string > keyValuePair in job.Arguments )
            {
                AddOriginSubSystem.AddOrigin( keyValuePair.Key, keyValuePair.Value );
            }
        }

        #endregion

    }

}
