using VisCPU.Instructions.Branch.Conditional;

namespace VisCPU.Instructions.Branch
{

    public class JumpToAddrInstruction : UnconditionalBranchInstruction
    {
        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 1;

        public override string Key => "JREF";

        #region Public

        public override void Process( Cpu cpu )
        {
            uint address = cpu.MemoryBus.Read( cpu.DecodeArgument( 0 ) );

            cpu.SetState( address - InstructionSize );
        }

        #endregion
    }

}
