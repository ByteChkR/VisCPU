using VisCPU.Instructions.Math;
using VisCPU.Utility.Logging;

namespace VisCPU.Instructions.Bitwise
{

    public abstract class BitwiseSelfInstruction : MathSelfInstruction
    {
        protected override LoggerSystems SubSystem => LoggerSystems.BitwiseInstructions;
    }

}
