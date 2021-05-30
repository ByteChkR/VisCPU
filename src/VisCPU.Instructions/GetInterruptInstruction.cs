namespace VisCPU.Instructions
{

    public class GetInterruptInstruction : BaseInstruction
    {

        public override uint Cycles => 1;

        public override string Key => "GINT";

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 1;

        #region Public

        public override void Process( Cpu cpu )
        {
            uint addr = cpu.DecodeArgument( 0 );

            uint val = cpu.GetInternalInterruptHandler();
            cpu.MemoryBus.Write( addr, val );
        }

        #endregion

    }

}