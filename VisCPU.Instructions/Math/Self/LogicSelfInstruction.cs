using VisCPU.Utility.Logging;

namespace VisCPU.Instructions.Math.Self
{

    public abstract class LogicSelfInstruction : MathSelfInstruction
    {
        protected override LoggerSystems SubSystem => LoggerSystems.LogicInstructions;
    }

}
