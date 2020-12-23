using VisCPU.Utility.Logging;

namespace VisCPU.Instructions.Branch.Conditional
{
    public abstract class BranchInstruction : Instruction
    {
        protected override LoggerSystems SubSystem => LoggerSystems.BranchInstructions;
    }
}