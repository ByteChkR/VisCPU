using VisCPU.Utility.Logging;

namespace VisCPU.Instructions.Memory
{

    public abstract class MemoryInstruction : Instruction
    {
        protected override LoggerSystems SubSystem => LoggerSystems.MemoryInstructions;
    }

}
