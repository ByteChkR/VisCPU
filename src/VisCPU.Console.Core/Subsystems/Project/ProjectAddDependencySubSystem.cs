using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Logging;
using VisCPU.Utility.ProjectSystem.Data;

namespace VisCPU.Console.Core.Subsystems.Project
{

    public class ProjectAddDependencySubSystem : ConsoleSubsystem
    {
        private class AddDependencyConfig
        {
            [field: Argument( Name = "name" )]
            public string Name { get; set; } = "ModuleDependency";

            [field: Argument( Name = "version" )]
            public string Version { get; set; } = "ANY";
        }

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public override void Help()
        {
            HelpSubSystem.WriteSubsystem( "vis dependency add <projectRoot>", new AddDependencyConfig() );
        }

        public override void Run( IEnumerable < string > args )
        {
            string[] a = args.ToArray();
            ProjectConfig t = ProjectConfig.Load( Path.Combine( Path.GetFullPath( a[0] ), "project.json" ) );

            AddDependencyConfig config = new AddDependencyConfig();
            ArgumentSyntaxParser.Parse( a.Skip( 1 ).ToArray(), config );

            t.Dependencies.Add(
                new ProjectDependency { ProjectName = config.Name, ProjectVersion = config.Version }
            );

            ProjectConfig.Save( Path.Combine( Directory.GetCurrentDirectory(), "project.json" ), t );
        }

        #endregion
    }

}
