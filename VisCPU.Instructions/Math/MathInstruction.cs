using VisCPU.Utility.Logging;

namespace VisCPU.Instructions.Math
{
    public abstract class MathInstruction : Instruction
    {
        protected override LoggerSystems SubSystem => LoggerSystems.MathInstructions;
    }
}