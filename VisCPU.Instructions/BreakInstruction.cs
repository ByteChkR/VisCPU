using VisCPU.Utility.Logging;

namespace VisCPU.Instructions
{
    public abstract class BaseInstruction:Instruction
    {

        protected override LoggerSystems SubSystem => LoggerSystems.BaseInstructions;

    }
    
    public class BreakInstruction : BaseInstruction
    {

        public override uint Cycles => 1;

        public override string Key => "BRK";

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 0;

        public override void Process(CPU cpu)
        {
            Log(cpu, "Set Break Flag");
            cpu.Set(CPU.Flags.BREAK);
        }

    }
}