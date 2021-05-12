﻿namespace VisCPU.Instructions.Memory
{

    public class CopyByReferenceInstruction : MemoryInstruction
    {

        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 2;

        public override string Key => "CREF";

        #region Public

        public override void Process( Cpu cpu )
        {
            uint arg0 = cpu.DecodeArgument( 0 );
            uint arg1 = cpu.DecodeArgument( 1 );
            uint addressSrc = cpu.MemoryBus.Read( arg0 );
            uint addressDst = cpu.MemoryBus.Read( arg1 );

            uint result = cpu.MemoryBus.Read( addressSrc );

            cpu.MemoryBus.Write( addressDst, result ); //Write back Result
        }

        #endregion

    }

}
