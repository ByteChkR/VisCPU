using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.Utility.Logging;

namespace VisCPU.Console.Core.Subsystems.Modules
{
    public class ModuleCreateSubSystem : ConsoleSubsystem
    {
        protected override LoggerSystems SubSystem => LoggerSystems.ModuleSystem;
        public override void Run(IEnumerable<string> args)
        {
            string[] a = args.ToArray();
            string path = a.Length != 0
                ? Path.Combine(Path.GetFullPath(a[0]), "project.json")
                : Path.Combine(Directory.GetCurrentDirectory(), "project.json");
            Log($"Writing Project Info: {path}");
            ModuleManager.CreateModuleTarget(path);
        }
    }
}