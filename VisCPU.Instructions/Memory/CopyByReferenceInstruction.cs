using System;

using VisCPU.Utility.Logging;

namespace VisCPU.Instructions.Memory
{

    public abstract class MemoryInstruction : Instruction
    {

        protected override LoggerSystems SubSystem => LoggerSystems.MemoryInstructions;

    }
    
    public class CopyByReferenceInstruction : MemoryInstruction
    {

        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 2;

        public override string Key => "CREF";

        public override void Process(CPU cpu)
        {
            uint arg0 = cpu.DecodeArgument(0);
            uint arg1 = cpu.DecodeArgument(1);
            uint addressSrc = cpu.MemoryBus.Read(arg0);
            uint addressDst = cpu.MemoryBus.Read(arg1);

            uint result = cpu.MemoryBus.Read(addressSrc);

            Log(
                           cpu,
                           $"0x{Convert.ToString(addressSrc, 16)}({Convert.ToString(result, 16)}) => 0x{Convert.ToString(addressDst, 16)}"
                          );

            cpu.MemoryBus.Write(addressDst, result); //Write back Result
        }

    }
}