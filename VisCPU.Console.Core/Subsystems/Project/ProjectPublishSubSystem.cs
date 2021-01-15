using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.Resolvers;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.Project
{

    public class ProjectPublishSubSystem : ConsoleSubsystem
    {

        public class PublishOptions
        {

            [Argument( Name = "origin" )]
            [Argument( Name = "o" )]
            public string Repository = "local";

        }

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public static void Publish(
            string root,
            PublishOptions publishOptions,
            ProjectPackSubSystem.PackOptions packOptions )
        {
            string src = Path.Combine( root, "build", "module.json" );

            ProjectPackSubSystem.Pack( root, packOptions );

            if ( !File.Exists( src ) )
            {
                EventManager < ErrorEvent >.SendEvent( new FileNotFoundEvent( src, false ) );

                return;
            }

            ProjectConfig t = ProjectConfig.Load( src );

            Logger.LogMessage( LoggerSystems.ModuleSystem, "Publishing '{0}'", src );

            ModuleResolver.GetManager( publishOptions.Repository ).
                           AddPackage(
                                      t,
                                      Path.Combine( root, "build", "module.zip" )
                                     );
        }

        public override void Help()
        {
            ProjectPackSubSystem.WriteHelp();

            HelpSubSystem.WriteSubsystem(
                                         "vis project publish <repo> <projectDir>",
                                         new ProjectPackSubSystem.PackOptions(),
                                         new PublishOptions()
                                        );
        }

        public override void Run( IEnumerable < string > args )
        {
            string[] a = args.ToArray();

            string root = a.Length != 0
                              ? Path.GetFullPath( a[1] )
                              : Directory.GetCurrentDirectory();

            ProjectPackSubSystem.PackOptions op = new ProjectPackSubSystem.PackOptions();
            PublishOptions pops = new PublishOptions();
            ArgumentSyntaxParser.Parse( args.ToArray(), op, pops );
            Publish( root, pops, op );
        }

        #endregion

    }

}
