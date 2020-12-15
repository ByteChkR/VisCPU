using System;

namespace VisCPU.Instructions.Memory
{
    public class LoadInstruction : MemoryInstruction
    {

        public override string Key => "LOAD";

        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 2;

        public override void Process(CPU cpu)
        {
            uint address = cpu.DecodeArgument(0);
            uint value = cpu.DecodeArgument(1);

            Log(
                           cpu,
                           $"0x{Convert.ToString(address, 16)} => 0x{Convert.ToString(value, 16)}"
                          );

            cpu.MemoryBus.Write(address, value); //Write back Result
        }

    }
}