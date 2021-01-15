using System.Collections.Generic;

using VisCPU.HL.Modules.BuildSystem;
using VisCPU.HL.Modules.Data;

namespace VisCPU.Console.Core.Subsystems.BuildSystem.JobRunner
{

    public class BuildJobMergedJobs : BuildJobRunner
    {

        public override string RunnerName => "merged";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job )
        {
            BuildJob includeJob = BuildJob.Load( job.Arguments["merge:include"] );

            foreach ( KeyValuePair < string, string > buildJobs in job.Arguments )
            {
                if ( buildJobs.Key != "merge:include" )
                {
                    includeJob.Arguments[buildJobs.Key] = buildJobs.Value;
                }
            }

            project.RunJob( projectRoot, target, includeJob, true );
        }

        #endregion

    }

}
