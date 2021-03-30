namespace VisCPU.Instructions.Branch.Conditional
{

    public class BranchIfLess : BranchInstruction
    {
        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 3;

        public override string Key => "BLT";

        #region Public

        public override bool Calculate( uint a, uint b )
        {
            return a < b;
        }

        #endregion
    }

}
