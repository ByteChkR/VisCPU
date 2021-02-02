﻿namespace VisCPU.Instructions.Bitwise
{

    public class BitwiseXOrInstruction : BitwiseInstruction
    {

        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 3;

        public override string Key => "XOR";

        #region Public

        public override uint Calculate( uint a, uint b )
        {
            return a ^ b;
        }

        #endregion

    }

}
