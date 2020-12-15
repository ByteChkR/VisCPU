using System;
using System.Collections.Generic;
using System.Linq;

using VisCPU.Utility.Events;

namespace VisCPU
{
    public class DuplicateInstructionOpCodesEvent : ErrorEvent
    {

        private const string EVENT_KEY = "instr-dup-op-code";
        public DuplicateInstructionOpCodesEvent(Instruction a, Instruction b, byte opCode) : base($"Instruction {a.Key} and {b.Key} share the same OpCode {opCode}", EVENT_KEY, false)
        {
        }

    }

    public class DuplicateVarDefinitionEvent : ErrorEvent
    {

        private const string EVENT_KEY = "var-duplicate-def";
        public DuplicateVarDefinitionEvent(string varName) : base($"Duplicate Definition of: {varName}", EVENT_KEY, false)
        {
        }

    }
    public class DuplicateConstVarDefinitionEvent : ErrorEvent
    {

        private const string EVENT_KEY = "var-duplicate-def";
        public DuplicateConstVarDefinitionEvent(string varName) : base($"Duplicate Definition of: {varName}", EVENT_KEY, false)
        {
        }

    }

    public class InstructionNotFoundEvent : ErrorEvent
    {

        private const string EVENT_KEY = "instr-op-not-found";
        public InstructionNotFoundEvent(byte opCode) : base($"Can not find Instruction with op code: {opCode}", EVENT_KEY, false)
        {
        }

        public InstructionNotFoundEvent(Instruction instruction) : base($"Can not find Instruction {instruction.Key}", EVENT_KEY, false)
        {
        }
        
        public InstructionNotFoundEvent(string key, int args) : base($"Can not find Instruction {key} with argument count {args}", EVENT_KEY, false)
        {
        }

    }

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