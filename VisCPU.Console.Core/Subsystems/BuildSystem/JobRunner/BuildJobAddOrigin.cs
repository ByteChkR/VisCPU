using System.Collections.Generic;

using VisCPU.Console.Core.Subsystems.Origins;
using VisCPU.HL.Modules.BuildSystem;
using VisCPU.HL.Modules.Data;

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
            BuildJob job )
        {
            foreach ( KeyValuePair < string, string > keyValuePair in job.Arguments )
            {
                AddOriginSubSystem.AddOrigin( keyValuePair.Key, keyValuePair.Value );
            }
        }

        #endregion

    }

}
