using System;

namespace VisCPU.Instructions.Math
{
    public class IncInstruction : MathInstruction
    {
        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 1;

        public override string Key => "INC";

        #region Public

        public override void Process(CPU cpu)
        {
            uint addressA = cpu.DecodeArgument(0); //Number A Address

            uint a = cpu.MemoryBus.Read(addressA); //Read Value From RAM

            uint result = a + 1; //Calculate Value

            Log(
                cpu,
                $"0x{Convert.ToString(addressA, 16)}({Convert.ToString(a, 16)}) + (1) = 0x{Convert.ToString(addressA, 16)}({Convert.ToString(result, 16)})"
            );

            cpu.MemoryBus.Write(addressA, result); //Write back Result
        }

        #endregion
    }
}