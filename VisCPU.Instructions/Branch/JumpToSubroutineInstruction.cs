using System;
using VisCPU.Instructions.Branch.Conditional;

namespace VisCPU.Instructions.Branch
{
    public class JumpToSubroutineInstruction : BranchInstruction
    {
        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 1;

        public override string Key => "JSR";

        #region Public

        public override void Process(CPU cpu)
        {
            uint address = cpu.DecodeArgument(0);

            Log(cpu, $"PC: 0x{Convert.ToString(address, 16)}");

            cpu.PushState(address - InstructionSize);
        }

        #endregion
    }
}