using VisCPU.Utility.Events;

namespace VisCPU.Events
{
    public class InstructionNotFoundEvent : ErrorEvent
    {
        private const string EVENT_KEY = "instr-op-not-found";

        #region Public

        public InstructionNotFoundEvent(byte opCode) : base(
            $"Can not find Instruction with op code: {opCode}",
            EVENT_KEY,
            false
        )
        {
        }

        public InstructionNotFoundEvent(Instruction instruction) : base(
            $"Can not find Instruction {instruction.Key}",
            EVENT_KEY,
            false
        )
        {
        }

        public InstructionNotFoundEvent(string key, int args) : base(
            $"Can not find Instruction {key} with argument count {args}",
            EVENT_KEY,
            false
        )
        {
        }

        #endregion
    }
}