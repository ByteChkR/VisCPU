namespace VisCPU.Instructions.Memory
{

    public class LoadInstruction : MemoryInstruction
    {

        public override string Key => "LOAD";

        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 2;

        #region Public

        public override void Process( Cpu cpu )
        {
            uint address = cpu.DecodeArgument( 0 );
            uint value = cpu.DecodeArgument( 1 );

            cpu.MemoryBus.Write( address, value ); //Write back Result
        }

        #endregion

    }

}
