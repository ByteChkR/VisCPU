using VisCPU.Utility.Logging;

namespace VisCPU.Instructions.Bitwise
{

    public abstract class BitwiseInstruction : Instruction
    {
        protected override LoggerSystems SubSystem => LoggerSystems.BitwiseInstructions;
    }

}
