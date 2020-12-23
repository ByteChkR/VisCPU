using System;
using VisCPU.Instructions.Branch.Conditional;

namespace VisCPU.Instructions.Branch
{
    public class JumpToSubroutineAddrInstruction : BranchInstruction
    {
        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 1;

        public override string Key => "JSREF";

        #region Public

        public override void Process(CPU cpu)
        {
            uint a = cpu.DecodeArgument(0);
            uint address = cpu.MemoryBus.Read(a);

            Log(cpu, $"PC: 0x{Convert.ToString(address, 16)}");

            cpu.PushState(address - InstructionSize);
        }

        #endregion
    }
}