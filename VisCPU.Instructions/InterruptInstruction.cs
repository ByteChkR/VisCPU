namespace VisCPU.Instructions
{

    public class InterruptInstruction : BaseInstruction
    {
        public override uint Cycles => 1;

        public override string Key => "INT";

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 1;

        #region Public

        public override void Process( Cpu cpu )
        {
            cpu.FireInterrupt( cpu.DecodeArgument( 0 ) );
        }

        #endregion
    }

}
