using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Logging;
using VisCPU.Utility.ProjectSystem.Data;
using VisCPU.Utility.ProjectSystem.Resolvers;

namespace VisCPU.Console.Core.Subsystems.Project
{

    public class ProjectRestoreSubSystem : ConsoleSubsystem
    {

        private class RestoreOptions
        {

            [field: Argument( Name = "origin" )]
            [field: Argument( Name = "o" )]
            public string Origin { get; set; } = "local";

        }

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public static void Restore( string projectRoot, string repo )
        {
            string src = Path.Combine( projectRoot, "project.json" );

            ProjectConfig t =
                ProjectConfig.Load( src );

            ProjectResolver.GetManager( repo ).Restore( t, projectRoot );
        }

        public override void Help()
        {
            HelpSubSystem.WriteSubsystem( "vis project restore", new RestoreOptions() );
        }

        public override void Run( IEnumerable < string > args )
        {
            string[] a = args.ToArray();

            string root = a.Length > 1
                              ? Path.GetFullPath( a[1] )
                              : Directory.GetCurrentDirectory();

            RestoreOptions options = new RestoreOptions();
            ArgumentSyntaxParser.Parse( a.Skip( 1 ).ToArray(), options );

            ProjectCleanSubSystem.Clean( root );

            Restore( root, options.Origin );
        }

        #endregion

    }

}
