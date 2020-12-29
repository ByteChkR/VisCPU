using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;

namespace VisCPU.Console.Core.Subsystems.Modules
{
    public class ModulePackSubSystem : ConsoleSubsystem
    {
        public override void Run(IEnumerable<string> args)
        {
            string[] a = args.ToArray();

            string src = a.Length != 0
                ? Path.Combine(Path.GetFullPath(a[0]), "project.json")
                : Path.Combine(Directory.GetCurrentDirectory(), "project.json");
            string outDir = Path.Combine(Path.GetDirectoryName(src), "build");
            if (Directory.Exists(outDir))
                Directory.Delete(outDir, true);

            ModuleTarget t = ModuleManager.LoadModuleTarget(src);

            Version v = Version.Parse(t.ModuleVersion);

            t.ModuleVersion = new Version(v.Major < 0 ? 0 : v.Major, v.Minor < 0 ? 0 : v.Minor+1,
                v.Build < 0 ? 0 : v.Build, v.Revision < 0 ? 0 : v.Revision).ToString();

            string temp = Path.Combine(
                Path.GetDirectoryName(Directory.GetCurrentDirectory()),
                "temp_" + t.ModuleName
            );
            Directory.CreateDirectory(temp);

            CopyTo(Path.GetDirectoryName(src), temp);

            File.Delete(Path.Combine(temp, "project.json"));
            foreach (ModuleDependency moduleDependency in t.Dependencies)
            {
                string p = Path.Combine(temp, moduleDependency.ModuleName);
                if (Directory.Exists(p))
                    Directory.Delete(p, true);
            }

            Directory.CreateDirectory(outDir);
            ZipFile.CreateFromDirectory(temp, Path.Combine(outDir, "module.zip"));
            Directory.Delete(temp, true);
            ModuleManager.SaveModuleTarget(t, Path.Combine(outDir, "module.json"));
            ModuleManager.SaveModuleTarget(t, src);
        }

        private void CopyTo(string src, string dst)
        {
            foreach (string dirPath in Directory.GetDirectories(src, "*",
                SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(src, dst));
            }

            foreach (string newPath in Directory.GetFiles(src, "*.*",
                SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(src, dst), true);
            }
        }
    }
}