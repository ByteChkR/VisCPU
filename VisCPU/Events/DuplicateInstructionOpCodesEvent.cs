using VisCPU.Utility.Events;

namespace VisCPU.Events
{
    public class DuplicateInstructionOpCodesEvent : ErrorEvent
    {
        private const string EVENT_KEY = "instr-dup-op-code";

        #region Public

        public DuplicateInstructionOpCodesEvent(Instruction a, Instruction b, byte opCode) : base(
            $"Instruction {a.Key} and {b.Key} share the same OpCode {opCode}",
            EVENT_KEY,
            false
        )
        {
        }

        #endregion
    }
}