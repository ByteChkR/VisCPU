namespace VisCPU.Instructions.Branch.Conditional
{

    public class BranchIfNotEqual : BranchInstruction
    {
        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 3;

        public override string Key => "BNE";

        #region Public

        public override void Process( CPU cpu )
        {
            uint a = cpu.MemoryBus.Read( cpu.DecodeArgument( 0 ) );
            uint b = cpu.MemoryBus.Read( cpu.DecodeArgument( 1 ) );
            uint address = cpu.DecodeArgument( 2 );

            bool jmp = a != b;

            if ( jmp )
            {
                cpu.SetState( address - InstructionSize );
            }
        }

        #endregion
    }

}
