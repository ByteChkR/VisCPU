using VisCPU.Utility.Logging;

namespace VisCPU.Instructions.Branch.Conditional
{

    public abstract class UnconditionalBranchInstruction : Instruction
    {
        protected override LoggerSystems SubSystem => LoggerSystems.BranchInstructions;
    }

}
