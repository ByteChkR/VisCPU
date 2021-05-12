using VisCPU.Instructions.Math.Self;

namespace VisCPU.Instructions.Math
{

    public class ShiftLeftInstruction : LogicInstruction
    {

        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 3;

        public override string Key => "SHL";

        #region Public

        public override uint Calculate( uint a, uint b )
        {
            return a << ( int ) b;
        }

        #endregion

    }

}
