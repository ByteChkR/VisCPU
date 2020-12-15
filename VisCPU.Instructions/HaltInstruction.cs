﻿namespace VisCPU.Instructions
{
    public class HaltInstruction : BaseInstruction
    {

        public override uint Cycles => 1;

        public override string Key => "HLT";

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 0;

        public override void Process(CPU cpu)
        {
            Log(cpu, "Set Halt Flag");
            cpu.Set(CPU.Flags.HALT);
        }

    }
}