using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.Modules
{

    public class ModuleCleanSubSystem : ConsoleSubsystem
    {

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public override void Run( IEnumerable < string > args )
        {
            string projectRoot = args.Any() ? Path.GetFullPath( args.First() ) : Directory.GetCurrentDirectory();

            if ( !File.Exists( Path.Combine( projectRoot, "project.json" ) ) )
            {
                throw new Exception( $"The folder '{projectRoot}' does not contain a 'project.json' file." );
            }

            if ( Directory.Exists( Path.Combine( projectRoot, "build" ) ) )
            {
                Log( "Deleting Project Build Directory" );
                Directory.Delete( Path.Combine( projectRoot, "build" ), true );
            }

            IEnumerable < string > sourceFiles = Directory.GetFiles( projectRoot, "*.*", SearchOption.AllDirectories ).
                                                           Select( Path.GetFullPath );

            int fcount = 0;

            foreach ( string sourceFile in sourceFiles )
            {
                if ( Path.GetExtension( sourceFile ) == ".vhl" || Path.GetExtension( sourceFile ) == ".json" )
                {
                    continue;
                }

                fcount++;
                File.Delete( sourceFile );
            }

            Log( "Deleted Files: " + fcount );

            ModuleTarget t = ModuleManager.LoadModuleTarget( Path.Combine( projectRoot, "project.json" ) );

            foreach ( ModuleDependency moduleDependency in t.Dependencies )
            {
                Log( "Deleting Dependency: " + moduleDependency.ModuleName );

                if ( Directory.Exists( Path.Combine( projectRoot, moduleDependency.ModuleName ) ) )
                {
                    Directory.Delete( Path.Combine( projectRoot, moduleDependency.ModuleName ), true );
                }
            }
        }

        #endregion

    }

}
