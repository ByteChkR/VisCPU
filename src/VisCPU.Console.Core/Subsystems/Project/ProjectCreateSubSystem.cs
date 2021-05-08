using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisCPU.Utility.Logging;
using VisCPU.Utility.ProjectSystem.BuildSystem;

namespace VisCPU.Console.Core.Subsystems.Project
{

    public class ProjectCreateSubSystem : ConsoleSubsystem
    {
        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public override void Help()
        {
            Log( "vis project new <projectRoot>" );
        }

        public override void Run( IEnumerable < string > args )
        {
            string[] a = args.ToArray();

            string path = a.Length != 0
                ? Path.GetFullPath( a[0] )
                : Directory.GetCurrentDirectory();

            Log( $"Writing Project Info: {path}" );

            CommonFiles.InitializeProjectFolder( path );
        }

        #endregion
    }

}
