using System.Collections.Generic;
using System.IO;
using VisCPU.Utility.ProjectSystem.BuildSystem;
using VisCPU.Utility.ProjectSystem.Data;
using VisCPU.Utility.ProjectSystem.Resolvers;

namespace VisCPU.Console.Core.Subsystems.BuildSystem.JobRunner
{

    public class BuildJobGetDependency : BuildJobRunner
    {
        public override string RunnerName => "get-dependency";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            ProjectBuildJob job )
        {
            foreach ( KeyValuePair < string, string > keyValuePair in job.Arguments )
            {
                ProjectConfig info = ProjectResolver.Resolve( keyValuePair.Key, keyValuePair.Value );
                info.Manager.Get( info, Path.Combine( projectRoot, info.ProjectName ) );
            }
        }

        #endregion
    }

}
