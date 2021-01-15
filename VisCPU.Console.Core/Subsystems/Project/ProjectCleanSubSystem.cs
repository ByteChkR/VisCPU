using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.HL.Modules.Data;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.Project
{

    public class ProjectCleanSubSystem : ConsoleSubsystem
    {

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public static void Clean( string projectRoot )
        {
            string src = Path.Combine( projectRoot, "project.json" );

            if ( !File.Exists( src ) )
            {
                EventManager<ErrorEvent>.SendEvent(new ProjectFileNotFoundEvent(projectRoot));
                return;
            }

            Logger.LogMessage( LoggerSystems.ModuleSystem, "Cleaning '{0}'", src );

            if ( Directory.Exists( Path.Combine( projectRoot, "build" ) ) )
            {
                Directory.Delete( Path.Combine( projectRoot, "build" ), true );
            }

            IEnumerable < string > sourceFiles = Directory.GetFiles( projectRoot, "*.*", SearchOption.AllDirectories ).
                                                           Select( Path.GetFullPath );

            int fcount = 0;

            foreach ( string sourceFile in sourceFiles )
            {
                string ext = Path.GetExtension( sourceFile );

                if ( ext == ".vbin" || ext == ".vbin.z" || ext == ".linkertext" )
                {
                    fcount++;
                    File.Delete( sourceFile );
                }
            }

            ProjectConfig t = ProjectConfig.Load( Path.Combine( projectRoot, "project.json" ) );

            foreach ( ProjectDependency moduleDependency in t.Dependencies )
            {
                if ( Directory.Exists( Path.Combine( projectRoot, moduleDependency.ProjectName ) ) )
                {
                    Directory.Delete( Path.Combine( projectRoot, moduleDependency.ProjectName ), true );
                }
            }
        }

        public override void Help()
        {
            Log( "vis project clean <projectRoot>" );
        }

        public override void Run( IEnumerable < string > args )
        {
            string projectRoot = args.Any() ? Path.GetFullPath( args.First() ) : Directory.GetCurrentDirectory();
            Clean( projectRoot );
        }

        #endregion

    }

}
