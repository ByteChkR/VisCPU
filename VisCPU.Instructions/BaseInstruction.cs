using VisCPU.Utility.Logging;

namespace VisCPU.Instructions
{

    public abstract class BaseInstruction : Instruction
    {
        protected override LoggerSystems SubSystem => LoggerSystems.BaseInstructions;
    }

}
