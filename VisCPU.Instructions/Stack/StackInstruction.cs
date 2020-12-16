using VisCPU.Utility.Logging;

namespace VisCPU.Instructions.Stack
{

    public abstract class StackInstruction : Instruction
    {

        protected override LoggerSystems SubSystem => LoggerSystems.StackInstructions;

    }

}