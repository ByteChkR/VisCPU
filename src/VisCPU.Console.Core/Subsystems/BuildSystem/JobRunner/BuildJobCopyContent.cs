using System.IO;
using VisCPU.Console.Core.Subsystems.Project;
using VisCPU.Utility.ProjectSystem.BuildSystem;
using VisCPU.Utility.ProjectSystem.Data;

namespace VisCPU.Console.Core.Subsystems.BuildSystem.JobRunner
{

    public class BuildJobCopyContent : BuildJobRunner
    {
        public override string RunnerName => "copy";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            ProjectBuildJob job )
        {
            string input = job.Arguments["source"];
            string output = job.Arguments["destination"];

            if ( Directory.Exists( input ) )
            {
                Directory.CreateDirectory( output );
                ProjectPackSubSystem.CopyTo( input, output );
            }
            else if ( File.Exists( input ) )
            {
                if ( Directory.Exists( output ) )
                {
                    File.Copy( input, Path.Combine( output, Path.GetFileName( input ) ), true );
                }
                else
                {
                    File.Copy( input, output, true );
                }
            }
        }

        #endregion
    }

}
