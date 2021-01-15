using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.HL.BuildSystem;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.Modules
{

    public class ProjectCreateSubSystem : ConsoleSubsystem
    {
        private class CreateProjectConfig
        {
            [Argument(Name = "name")]
            public string Name = "MyProject";
            [Argument(Name = "version")]
            public string Version = "0.0.0.1";

        }

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public override void Run( IEnumerable < string > args )
        {
            string[] a = args.ToArray();

            string path = a.Length != 0
                              ? Path.Combine( Path.GetFullPath( a[0] ), "project.json" )
                              : Path.Combine( Directory.GetCurrentDirectory(), "project.json" );

            Log( $"Writing Project Info: {path}" );
            CreateProjectConfig config = new CreateProjectConfig();
            ArgumentSyntaxParser.Parse(a.Skip(1).ToArray(), config);

            ProjectConfig.Save(
                               path,
                               new ProjectConfig( null, config.Name, config.Version, new ProjectDependency[0] )
                              );
            ModuleManager.CreateModuleTarget( path );
        }

        public override void Help()
        {
            HelpSubSystem.WriteSubsystem( "vis project create <projectRoot>", new CreateProjectConfig() );
        }

        #endregion

    }

}
