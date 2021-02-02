using System.IO;

using VisCPU.Utility.ProjectSystem.BuildSystem;
using VisCPU.Utility.ProjectSystem.Data;

namespace VisCPU.Console.Core.Subsystems.BuildSystem.JobRunner
{

    public class BuildJobExternalBuild : BuildJobRunner
    {

        public override string RunnerName => "external-build";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            ProjectBuildJob job )
        {
            string path = job.Arguments["path"];
            job.Arguments.TryGetValue( "target", out string externalTarget );
            ProjectConfig external = ProjectConfig.Load( path );
            external.RunTarget( Path.GetDirectoryName( path ), externalTarget );
        }

        #endregion

    }

}
