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

        public static void Clean( string projectRoot )
        {
            if (!File.Exists(Path.Combine(projectRoot, "project.json")))
            {
                throw new Exception($"The folder '{projectRoot}' does not contain a 'project.json' file.");
            }

            if (Directory.Exists(Path.Combine(projectRoot, "build")))
            {
                Directory.Delete(Path.Combine(projectRoot, "build"), true);
            }

            IEnumerable<string> sourceFiles = Directory.GetFiles(projectRoot, "*.*", SearchOption.AllDirectories).
                                                        Select(Path.GetFullPath);

            int fcount = 0;

            foreach (string sourceFile in sourceFiles)
            {
                string ext = Path.GetExtension(sourceFile);

                if (ext == ".vbin" || ext == ".vbin.z" || ext == ".linkertext")
                {
                    fcount++;
                    File.Delete(sourceFile);
                }
            }
            

            ModuleTarget t = ModuleManager.LoadModuleTarget(Path.Combine(projectRoot, "project.json"));

            foreach (ModuleDependency moduleDependency in t.Dependencies)
            {

                if (Directory.Exists(Path.Combine(projectRoot, moduleDependency.ModuleName)))
                {
                    Directory.Delete(Path.Combine(projectRoot, moduleDependency.ModuleName), true);
                }
            }
        }

        public override void Run( IEnumerable < string > args )
        {
            string projectRoot = args.Any() ? Path.GetFullPath( args.First() ) : Directory.GetCurrentDirectory();
            Clean( projectRoot );

        }

        #endregion

    }

}
