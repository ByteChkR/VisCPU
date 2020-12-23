using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.HL.Modules.Resolvers;

namespace VisCPU.Console.Core.Subsystems.Modules
{
    public class ModuleRestoreSubSystem : ConsoleSubsystem
    {
        public override void Run(IEnumerable<string> args)
        {
            string[] a = args.ToArray();

            string root = a.Length != 0
                ? Path.GetFullPath(a[0])
                : Directory.GetCurrentDirectory();

            string src = Path.Combine(root, "project.json");
            ModuleTarget t =
                ModuleManager.LoadModuleTarget(src);

            ModuleResolver.Manager.Restore(t, root);
        }
    }
}