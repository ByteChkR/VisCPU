using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.HL.BuildSystem;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.HL.Modules.Resolvers;
using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.Modules
{

    public class ProjectRestoreSubSystem : ConsoleSubsystem
    {
        private class RestoreOptions
        {

            [Argument(Name = "origin")]
            [Argument(Name = "o")]
            public string Origin = "local";

        }

        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;

        #region Public

        public static void Restore(string projectRoot, string repo)
        {
            string src = Path.Combine(projectRoot, "project.json");

            ProjectConfig t =
                ProjectConfig.Load(src);

            ModuleResolver.GetManager(repo).Restore(t, projectRoot);
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

        public override void Help()
        {
            HelpSubSystem.WriteSubsystem( "vis project restore", new RestoreOptions() );
        }

        #endregion

    }

}
