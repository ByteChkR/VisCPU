﻿namespace VisCPU.Instructions.Branch.Conditional
{

    public class BranchIfZero : BranchIfEqual
    {

        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 2;

        public override string Key => "BEZ";

        #region Public

        public override void Process( Cpu cpu )
        {
            uint a = cpu.MemoryBus.Read( cpu.DecodeArgument( 0 ) );
            uint address = cpu.DecodeArgument( 1 );

            bool jmp = Calculate( a, 0 );

            if ( jmp )
            {
                cpu.SetState( address - InstructionSize );
            }
        }

        #endregion

    }

}
