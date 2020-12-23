using System;

namespace VisCPU.Instructions.Memory
{
    public class CopyInstruction : MemoryInstruction
    {
        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 2;

        public override string Key => "COPY";

        #region Public

        public override void Process(CPU cpu)
        {
            uint addressSrc = cpu.DecodeArgument(0);
            uint addressDst = cpu.DecodeArgument(1);

            uint result = cpu.MemoryBus.Read(addressSrc);

            Log(
                cpu,
                $"0x{Convert.ToString(addressSrc, 16)}({Convert.ToString(result, 16)}) => 0x{Convert.ToString(addressDst, 16)}"
            );

            cpu.MemoryBus.Write(addressDst, result); //Write back Result
        }

        #endregion
    }
}