using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;

namespace VisCPU.Console.Core.Subsystems.Modules
{
    public class ModuleCleanSubSystem : ConsoleSubsystem
    {
        public override void Run(IEnumerable<string> args)
        {
            string projectRoot = args.Any() ? Path.GetFullPath(args.First()) : Directory.GetCurrentDirectory();
            if (Directory.Exists(Path.Combine(projectRoot, "build")))
            {
                Directory.Delete(Path.Combine(projectRoot, "build"), true);
            }


            IEnumerable<string> sourceFiles = Directory.GetFiles(projectRoot, "*.*", SearchOption.AllDirectories)
                .Select(Path.GetFullPath);

            foreach (string sourceFile in sourceFiles)
            {
                if (Path.GetExtension(sourceFile) == ".vhl" || Path.GetExtension(sourceFile) == ".json")
                {
                    continue;
                }


                Log("Deleting File: " + sourceFile);
                File.Delete(sourceFile);
            }

            ModuleTarget t = ModuleManager.LoadModuleTarget(Path.Combine(projectRoot, "project.json"));

            foreach (ModuleDependency moduleDependency in t.Dependencies)
            {
                Log("Deleting Dependency: " + moduleDependency.ModuleName);
                if (Directory.Exists(Path.Combine(projectRoot, moduleDependency.ModuleName)))
                {
                    Directory.Delete(Path.Combine(projectRoot, moduleDependency.ModuleName), true);
                }
            }

        }
    }
}