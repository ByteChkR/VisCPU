using System;

using VisCPU.Instructions.Branch.Conditional;

namespace VisCPU.Instructions.Branch
{

    public class JumpToInstruction : BranchInstruction
    {

        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 1;

        public override string Key => "JMP";

        #region Public

        public override void Process( CPU cpu )
        {
            uint address = cpu.DecodeArgument( 0 );
            

            cpu.SetState( address - InstructionSize );
        }

        #endregion

    }

}
