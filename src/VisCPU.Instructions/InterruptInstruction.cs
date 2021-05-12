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

    public class ClearStackInstruction : BaseInstruction
    {

        public override uint Cycles => 1;

        public override string Key => "CLRS";

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 0;

        #region Public

        public override void Process( Cpu cpu )
        {
            cpu.ClearStackAndStates();
        }

        #endregion

    }

}
