﻿using System;
using System.Collections.Generic;
using System.Linq;

using VisCPU.Utility.Events;

namespace VisCPU
{

    public abstract class InstructionSet
    {

        private readonly Instruction[] instructions;

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
                    EventManager < ErrorEvent >.SendEvent(
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

        public abstract string SetKey { get; }

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
            Instruction i= instructions[opCode];

            if ( i == null )
            {
                EventManager < ErrorEvent >.SendEvent( new InstructionNotFoundEvent( (byte)opCode ) );
            }

            return i;
        }

        public struct InstructionData
        {

            public Instruction instruction;
            public byte OpCode;

        }

    }
}