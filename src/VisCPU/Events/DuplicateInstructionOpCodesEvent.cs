using VisCPU.Instructions;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Events
{

    internal class DuplicateInstructionOpCodesEvent : ErrorEvent
    {
        #region Public

        public DuplicateInstructionOpCodesEvent( Instruction a, Instruction b, byte opCode ) : base(
            $"Instruction {a.Key} and {b.Key} share the same OpCode {opCode}",
            ErrorEventKeys.s_InstrDuplivateOpCode,
            false
        )
        {
        }

        #endregion
    }

}
