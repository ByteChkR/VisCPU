using System.Collections.Generic;
using VisCPU.ProjectSystem.BuildSystem;
using VisCPU.ProjectSystem.Data;

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
            ProjectBuildJob job )
        {
            foreach ( KeyValuePair < string, string > buildJobs in job.Arguments )
            {
                ProjectBuildJob subJob = ProjectBuildJob.Load( buildJobs.Value );
                project.RunJob( projectRoot, target, subJob, false );
            }
        }

        #endregion
    }

}
