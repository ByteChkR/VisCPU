namespace VisCPU.Instructions
{

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