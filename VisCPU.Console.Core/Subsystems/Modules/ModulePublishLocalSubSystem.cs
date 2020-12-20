using System.Collections.Generic;
using System.IO;

using VisCPU.HL.Modules;

namespace VisCPU.Console.Core.Subsystems
{

    public class ModulePublishLocalSubSystem : ConsoleSubsystem
    {
        public override void Run(IEnumerable<string> args)
        {
            string pDir = Path.Combine(Directory.GetCurrentDirectory(), "build", "module.json");
            ModuleTarget t =
                ModuleManager.LoadModuleTarget(pDir);
            ModuleResolver.Manager.AddPackage(
                                              t,
                                              Path.Combine(Directory.GetCurrentDirectory(), "build", "module.zip")
                                             );
        }
    }

}