using System.Collections.Generic;
using System.IO;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;
using VisCPU.HL.Modules.Resolvers;

namespace VisCPU.Console.Core.Subsystems.Modules
{

    public class ModuleRestoreSubSystem : ConsoleSubsystem
    {

        public override void Run( IEnumerable < string > args )
        {
            string pDir = Path.Combine(Directory.GetCurrentDirectory(), "project.json");
            ModuleTarget t =
                ModuleManager.LoadModuleTarget(pDir);

            ModuleResolver.Manager.Restore(t, Directory.GetCurrentDirectory());
        }

    }

}