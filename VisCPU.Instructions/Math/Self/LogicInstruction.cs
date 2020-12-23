using VisCPU.Utility.Logging;

namespace VisCPU.Instructions.Math.Self
{
    public abstract class LogicInstruction : Instruction
    {
        protected override LoggerSystems SubSystem => LoggerSystems.LogicInstructions;
    }
}