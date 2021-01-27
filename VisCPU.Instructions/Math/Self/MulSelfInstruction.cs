namespace VisCPU.Instructions.Math.Self
{

    public class MulSelfInstruction : MathSelfInstruction
    {
        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 2;

        public override string Key => "MUL";

        #region Public

        public override uint Calculate( uint a, uint b )
        {
            return a * b;
        }

        #endregion
    }

}
