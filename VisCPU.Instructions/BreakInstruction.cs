﻿namespace VisCPU.Instructions
{

    public class BreakInstruction : BaseInstruction
    {

        public override uint Cycles => 1;

        public override string Key => "BRK";

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 0;

        #region Public

        public override void Process( CPU cpu )
        {
            cpu.Set( CPU.Flags.BREAK );
        }

        #endregion

    }

}
