using System.Collections.Generic;

using VisCPU.HL.Modules.BuildSystem;
using VisCPU.HL.Modules.Data;

namespace VisCPU.Console.Core.Subsystems.BuildSystem.JobRunner
{

    public class BuildJobCombinedJobs : BuildJobRunner
    {

        public override string RunnerName => "combined";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job )
        {
            foreach ( KeyValuePair < string, string > buildJobs in job.Arguments )
            {
                BuildJob subJob = BuildJob.Load( buildJobs.Value );
                project.RunJob( projectRoot, target, subJob );
            }
        }

        #endregion

    }

}
