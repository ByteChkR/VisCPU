using System.Collections.Generic;
using System.IO;

using VisCPU.HL.Modules.BuildSystem;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.Resolvers;

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
            BuildJob job )
        {
            foreach ( KeyValuePair < string, string > keyValuePair in job.Arguments )
            {
                ProjectConfig info = ModuleResolver.Resolve( keyValuePair.Key, keyValuePair.Value );
                info.Manager.Get( info, Path.Combine( projectRoot, info.ProjectName ) );
            }
        }

        #endregion

    }

}
