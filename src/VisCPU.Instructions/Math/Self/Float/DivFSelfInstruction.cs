﻿namespace VisCPU.Instructions.Math.Self.Float
{

    public class DivFSelfInstruction : MathFSelfInstruction
    {

        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 2;

        public override string Key => "DIV.F";

        #region Public

        public override float Calculate( float a, float b )
        {
            return a / b;
        }

        #endregion

    }

}
