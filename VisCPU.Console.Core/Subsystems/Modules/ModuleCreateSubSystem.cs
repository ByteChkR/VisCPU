using System.Collections.Generic;
using System.IO;

using VisCPU.HL.Modules;

namespace VisCPU.Console.Core.Subsystems
{

    public class ModuleCreateSubSystem : ConsoleSubsystem
    {

        public override void Run(IEnumerable<string> args)
        {
            Log( $"Writing Project Info: {Path.Combine( Directory.GetCurrentDirectory(), "project.json" )}" );
            ModuleManager.CreateModuleTarget(Path.Combine(Directory.GetCurrentDirectory(), "project.json"));
        }

    }

}