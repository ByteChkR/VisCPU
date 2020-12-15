﻿using VisCPU.Utility.Logging;

namespace VisCPU.Instructions.Stack
{

    public abstract class StackInstruction : Instruction
    {

        protected override LoggerSystems SubSystem => LoggerSystems.StackInstructions;

    }
    public class PeekInstruction : StackInstruction
    {

        public override uint Cycles => 1;

        public override string Key => "PEEK";

        public override uint InstructionSize => 4;

        public override uint ArgumentCount => 1;

        public override void Process(CPU cpu)
        {
            uint addr = cpu.DecodeArgument(0);

            uint val = cpu.Peek();
            Log(cpu, $"Peeking Value: {val}");
            cpu.MemoryBus.Write(addr, val);
        }

    }
}