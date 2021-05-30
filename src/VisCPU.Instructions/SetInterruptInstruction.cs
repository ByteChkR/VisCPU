namespace VisCPU.Instructions
{

    public class SetInterruptInstruction : BaseInstruction
    {

        public override uint Cycles => 1;

        public override string Key => "SINT";

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 1;

        #region Public

        public override void Process( Cpu cpu )
        {
            cpu.SetInternalInterruptHandler( cpu.MemoryBus.Read( cpu.DecodeArgument( 0 ) ) );
        }

        #endregion

    }

}