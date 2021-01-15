using System.IO;

using VisCPU.HL.Modules.BuildSystem;
using VisCPU.HL.Modules.Data;

namespace VisCPU.Console.Core.Subsystems.BuildSystem.JobRunner
{

    public class BuildJobMoveContent : BuildJobRunner
    {

        public override string RunnerName => "move";

        #region Public

        public override void RunJob(
            string projectRoot,
            ProjectConfig project,
            ProjectBuildTarget target,
            BuildJob job )
        {
            string input = job.Arguments["source"];
            string output = job.Arguments["destination"];

            if ( Directory.Exists( input ) )
            {
                if ( Directory.Exists( output ) )
                {
                    Directory.Delete( output );
                }

                Directory.Move( input, output );
            }
            else if ( File.Exists( input ) )
            {
                if ( Directory.Exists( output ) )
                {
                    string path = Path.Combine( output, Path.GetFileName( input ) );

                    if ( File.Exists( path ) )
                    {
                        File.Delete( path );
                    }

                    File.Move( input, path );
                }
                else
                {
                    if ( File.Exists( output ) )
                    {
                        File.Delete( output );
                    }

                    File.Move( input, output );
                }
            }
        }

        #endregion

    }

}
