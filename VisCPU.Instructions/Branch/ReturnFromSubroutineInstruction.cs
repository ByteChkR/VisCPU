using VisCPU.Instructions.Branch.Conditional;

namespace VisCPU.Instructions.Branch
{
    public class ReturnFromSubroutineInstruction : BranchInstruction
    {

        public override uint Cycles => 1;

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 0;

        public override string Key => "RET";

        public override void Process(CPU cpu)
        {
            cpu.PopState();

            Log(cpu, "Returning from Subroutine.");
        }

    }
}