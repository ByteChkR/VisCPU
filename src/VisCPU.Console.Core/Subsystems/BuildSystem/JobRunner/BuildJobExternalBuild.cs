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
            string[] projects = path.Split( ';' );
            job.Arguments.TryGetValue("target", out string externalTarget);

            foreach ( string projJson in projects )
            {
                if(string.IsNullOrWhiteSpace(projJson))continue;

                string projectPath = Path.GetFullPath(projJson);
                ProjectConfig external = ProjectConfig.Load(projectPath);
                external.RunTarget(Path.GetDirectoryName(projectPath), externalTarget);
            }
        }

        #endregion

    }

}
