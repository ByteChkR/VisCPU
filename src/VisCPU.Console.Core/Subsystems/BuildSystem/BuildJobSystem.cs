using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisCPU.Console.Core.Subsystems.BuildSystem.JobRunner;
using VisCPU.ProjectSystem.BuildSystem;
using VisCPU.ProjectSystem.Data;
using VisCPU.ProjectSystem.Resolvers;

namespace VisCPU.Console.Core.Subsystems.BuildSystem
{

    public class BuildJobSystem : ConsoleSubsystem
    {
        #region Public

        public override void Help()
        {
            Log( "vis make <root> <target>" );
        }

        public override void Run( IEnumerable < string > args )
        {
            ProjectResolver.Initialize();
            CommonFiles.GenerateCommonFiles();
            string[] a = args.ToArray();

            string root = a.Length != 0
                ? Path.GetFullPath( a[0] )
                : Directory.GetCurrentDirectory();

            string src = Path.Combine( root, "project.json" );

            ProjectConfig.AddRunner( new BuildJobBuild() );
            ProjectConfig.AddRunner( new BuildJobClean() );
            ProjectConfig.AddRunner( new BuildJobPublish() );
            ProjectConfig.AddRunner( new BuildJobRestore() );
            ProjectConfig.AddRunner( new BuildJobExternalBuild() );
            ProjectConfig.AddRunner( new BuildJobCombinedJobs() );
            ProjectConfig.AddRunner( new BuildJobMoveContent() );
            ProjectConfig.AddRunner( new BuildJobCopyContent() );
            ProjectConfig.AddRunner( new BuildJobRunJob() );
            ProjectConfig.AddRunner( new BuildJobGetDependency() );
            ProjectConfig.AddRunner( new BuildJobAddOrigin() );
            ProjectConfig.AddRunner( new BuildJobRemoveOrigin() );
            ProjectConfig.AddRunner( new BuildJobMergedJobs() );

            ProjectConfig config = ProjectConfig.Load( src );
            string target = config.DefaultTarget;

            if ( a.Length > 1 )
            {
                target = a[1];
            }

            config.RunTarget( root, target );
        }

        #endregion
    }

}
