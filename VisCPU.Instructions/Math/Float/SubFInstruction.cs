namespace VisCPU.Instructions.Math.Float
{

    public class SubFInstruction : MathFInstruction
    {
        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 3;

        public override string Key => "SUB.F";

        #region Public

        public override float Calculate( float a, float b )
        {
            return a - b;
        }

        #endregion
    }

}
