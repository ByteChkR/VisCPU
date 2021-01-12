namespace VisCPU.Instructions
{

    public class NoOpInstruction : BaseInstruction
    {

        public override uint Cycles => 1;

        public override string Key => "NOP";

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 0;

        #region Public

        public override void Process( CPU cpu )
        {
            Log( "No Operation" );
        }

        #endregion

    }

}
