using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;

namespace VisCPU.Console.Core.Subsystems.Modules
{
    public class ModuleAddDependencySubSystem : ConsoleSubsystem
    {
        public override void Run(IEnumerable<string> args)
        {
            string[] a = args.ToArray();
            ModuleTarget t;
            t = ModuleManager.LoadModuleTarget(a.Length != 0
                ? Path.Combine(Path.GetFullPath(a[0]), "project.json")
                : Path.Combine(Directory.GetCurrentDirectory(), "project.json"));
            t.Dependencies.Add(
                new ModuleDependency
                {
                    ModuleName = "ModuleDependency",
                    ModuleVersion = "ANY"
                }
            );

            ModuleManager.SaveModuleTarget(t, Path.Combine(Directory.GetCurrentDirectory(), "project.json"));
        }
    }
}