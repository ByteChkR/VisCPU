using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.HL.Modules.Resolvers;

namespace VisCPU.Console.Core.Subsystems.Modules
{
    public class ModulePublishLocalSubSystem : ConsoleSubsystem
    {
        public override void Run(IEnumerable<string> args)
        {
            string[] a = args.ToArray();

            string root = a.Length != 0
                ? Path.GetFullPath(a[0])
                : Directory.GetCurrentDirectory();

            string src = Path.Combine(root, "build", "module.json");

            ModuleTarget t = ModuleManager.LoadModuleTarget(src);

            ModuleResolver.Manager.AddPackage(
                t,
                Path.Combine(root, "build", "module.zip")
            );
        }
    }
}