using VisCPU.Console.Core.Subsystems.Project;
using VisCPU.ProjectSystem.BuildSystem;
using VisCPU.ProjectSystem.Data;

namespace VisCPU.Console.Core.Subsystems.BuildSystem.JobRunner
{

    public class BuildJobPublish : BuildJobRunner
    {
        public override string RunnerName => "publish";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            ProjectBuildJob job )
        {
            ProjectPackSubSystem.PackOptions opts = new ProjectPackSubSystem.PackOptions();
            ProjectPublishSubSystem.PublishOptions pops = new ProjectPublishSubSystem.PublishOptions();

            if ( job.Arguments.TryGetValue( "version", out string ver ) )
            {
                opts.VersionString = ver;
            }

            if ( job.Arguments.TryGetValue( "origin", out string origin ) )
            {
                pops.Repository = origin;
            }

            ProjectPublishSubSystem.Publish( projectRoot, pops, opts );
        }

        #endregion
    }

}
