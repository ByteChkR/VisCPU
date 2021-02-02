namespace VisCPU.Instructions.Branch.Conditional
{

    public abstract class BranchInstruction : UnconditionalBranchInstruction
    {
        #region Public

        public abstract bool Calculate( uint a, uint b );

        public override void Process( Cpu cpu )
        {
            uint a = cpu.MemoryBus.Read( cpu.DecodeArgument( 0 ) );
            uint b = cpu.MemoryBus.Read( cpu.DecodeArgument( 1 ) );
            uint address = cpu.DecodeArgument( 2 );

            bool jmp = Calculate( a, b );

            if ( jmp )
            {
                cpu.SetState( address - InstructionSize );
            }
        }

        #endregion
    }

}
