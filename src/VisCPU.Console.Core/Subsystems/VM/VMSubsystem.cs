using System.Collections.Generic;

namespace VisCPU.Console.Core.Subsystems.VM
{

    public class VMSubsystem : ConsoleSystem
    {
        public override Dictionary < string, ConsoleSubsystem > SubSystems { get; } =
            new Dictionary < string, ConsoleSubsystem > { { "start", new VMStartSubSystem() } };
    }

}