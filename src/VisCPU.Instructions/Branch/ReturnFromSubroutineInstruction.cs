using VisCPU.Instructions.Branch.Conditional;

namespace VisCPU.Instructions.Branch
{

    public class ReturnFromSubroutineInstruction : UnconditionalBranchInstruction
    {
        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 0;

        public override string Key => "RET";

        #region Public

        public override void Process( Cpu cpu )
        {
            cpu.PopState();
        }

        #endregion
    }

}
