using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

using VisCPU.HL.Modules;

namespace VisCPU.Console.Core.Subsystems
{

    public class ModulePackSubSystem : ConsoleSubsystem
    {

        public override void Run(IEnumerable<string> args)
        {
            string pDir = Path.Combine( Directory.GetCurrentDirectory(), "project.json" );
            ModuleTarget t =
                ModuleManager.LoadModuleTarget(pDir);

            string temp = Path.Combine(
                                       Path.GetDirectoryName(Directory.GetCurrentDirectory()),
                                       "temp_" + t.ModuleName
                                      );

            Directory.CreateDirectory( temp );

            CopyTo(Directory.GetCurrentDirectory(), temp );

            File.Delete( Path.Combine( temp, "project.json" ) );
            foreach ( ModuleDependency moduleDependency in t.Dependencies )
            {
                Directory.Delete( Path.Combine( temp, moduleDependency.ModuleName ) , true);
            }

            string outDir = Path.Combine( Directory.GetCurrentDirectory(), "build" );
            Directory.CreateDirectory(outDir);
            ZipFile.CreateFromDirectory( temp, Path.Combine( outDir, "module.zip" ) );
            Directory.Delete( temp, true );
            ModuleManager.SaveModuleTarget( t, Path.Combine( outDir, "module.json" ) );
        }

        private void CopyTo(string src, string dst)
        {
            foreach (string dirPath in Directory.GetDirectories(src, "*",
                                                                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(src, dst));
            
            foreach (string newPath in Directory.GetFiles(src, "*.*",
                                                          SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(src, dst), true);
        }

    }

}