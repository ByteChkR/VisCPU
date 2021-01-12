using System;

namespace VisCPU.Instructions.Memory
{

    public class DeReferenceInstruction : MemoryInstruction
    {

        public override uint Cycles => 1;

        public override string Key => "DREF";

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 2;

        #region Public

        public override void Process( CPU cpu )
        {
            uint addressSrcPtr = cpu.DecodeArgument( 0 );
            uint addressSrc = cpu.MemoryBus.Read( cpu.MemoryBus.Read( addressSrcPtr ) ); // Dereference
            uint addressDst = cpu.DecodeArgument( 1 );
            

            cpu.MemoryBus.Write( addressDst, addressSrc ); //Write back Result
        }

        #endregion

    }

}
