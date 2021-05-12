using VisCPU.Utility.Logging;

namespace VisCPU.Instructions.Math.Self
{

    public abstract class LogicInstruction : MathInstruction
    {

        protected override LoggerSystems SubSystem => LoggerSystems.LogicInstructions;

    }

}
