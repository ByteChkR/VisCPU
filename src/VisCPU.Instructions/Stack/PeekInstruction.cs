namespace VisCPU.Instructions.Stack
{

    public class PeekInstruction : StackInstruction
    {
        public override uint Cycles => 1;

        public override string Key => "PEEK";

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 1;

        #region Public

        public override void Process( Cpu cpu )
        {
            uint addr = cpu.DecodeArgument( 0 );

            uint val = cpu.Peek();
            cpu.MemoryBus.Write( addr, val );
        }

        #endregion
    }

}
