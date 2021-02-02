namespace VisCPU.Instructions.Bitwise
{

    public class BitwiseOrInstruction : BitwiseInstruction
    {

        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 3;

        public override string Key => "OR";

        #region Public

        public override uint Calculate( uint a, uint b )
        {
            return a | b;
        }

        #endregion

    }

}
