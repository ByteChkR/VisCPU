using System.Collections.Generic;
using System.IO;
using VisCPU.HL.Modules.Data;
using VisCPU.HL.Modules.ModuleManagers;

namespace VisCPU.Console.Core.Subsystems.Modules
{
    public class ModuleAddDependencySubSystem : ConsoleSubsystem
    {

        public override void Run(IEnumerable<string> args)
        {
            ModuleTarget t =
                ModuleManager.LoadModuleTarget( Path.Combine( Directory.GetCurrentDirectory(), "project.json" ) );

            t.Dependencies.Add(
                               new ModuleDependency
                               {
                                   ModuleName = "ModuleDependency",
                                   ModuleVersion = "ANY"
                               }
                              );

            ModuleManager.SaveModuleTarget( t, Path.Combine( Directory.GetCurrentDirectory(), "project.json" ) );
        }

    }

}