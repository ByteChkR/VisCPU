﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.Logging;
using VisCPU.Utility.ProjectSystem.Data;
using VisCPU.Utility.ProjectSystem.Resolvers;

namespace VisCPU.Console.Core.Subsystems.Project
{

    public class ProjectPublishSubSystem : ConsoleSubsystem
    {
        public class PublishOptions
        {
            [field: Argument( Name = "origin" )]
            [field: Argument( Name = "o" )]
            public string Repository { get; set; } = "local";
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

            ProjectResolver.GetManager( publishOptions.Repository ).
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
