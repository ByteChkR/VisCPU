﻿using System.Collections.Generic;
using System.Linq;
using VisCPU.Events;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU
{
    public abstract class InstructionSet
    {
        private readonly Instruction[] instructions;

        public abstract string SetKey { get; }

        public struct InstructionData
        {
            public Instruction instruction;
            public byte OpCode;
        }

        #region Public

        public Instruction[] GetInstructions() => instructions.ToArray();
        public Instruction[] GetInstructions( string key )
        {
            return instructions.Where(x => x.Key == key).ToArray();
        }

        public Instruction GetInstruction(string key, int arguments)
        {
            Instruction ret = instructions.FirstOrDefault(x => x.Key == key && x.ArgumentCount == arguments);

            if (ret == null)
            {
                EventManager<ErrorEvent>.SendEvent(new InstructionNotFoundEvent(key, arguments));
            }

            return ret;
        }

        public uint GetInstruction(Instruction instr)
        {
            int idx = instructions.ToList().IndexOf(instr);

            if (idx == -1)
            {
                EventManager<ErrorEvent>.SendEvent(new InstructionNotFoundEvent(instr));
            }

            return (uint) idx; //lazy bastard
        }

        public Instruction GetInstruction(uint opCode)
        {
            Instruction i = instructions[opCode];

            if (i == null)
            {
                EventManager<ErrorEvent>.SendEvent(new InstructionNotFoundEvent((byte) opCode));
            }

            return i;
        }

        #endregion

        #region Protected

        protected InstructionSet(Instruction[] instructions, Instruction noOp = null)
        {
            this.instructions = instructions;
        }

        protected InstructionSet(IEnumerable<InstructionData> data, Instruction noOp = null)
        {
            instructions = new Instruction[byte.MaxValue];

            foreach (InstructionData instructionData in data)
            {
                if (instructions[instructionData.OpCode] == null)
                {
                    instructions[instructionData.OpCode] = instructionData.instruction;
                }
                else
                {
                    EventManager<ErrorEvent>.SendEvent(
                        new DuplicateInstructionOpCodesEvent(
                            instructions[instructionData.OpCode],
                            instructionData.instruction,
                            instructionData.OpCode
                        )
                    );
                }
            }

            if (noOp != null)
            {
                for (int i = 0; i < instructions.Length; i++)
                {
                    if (instructions[i] == null)
                    {
                        instructions[i] = noOp;
                    }
                }
            }
        }

        #endregion
    }
}