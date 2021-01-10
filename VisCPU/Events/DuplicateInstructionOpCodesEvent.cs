using VisCPU.Instructions;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Events
{

    public class DuplicateInstructionOpCodesEvent : ErrorEvent
    {

        #region Public

        public DuplicateInstructionOpCodesEvent( Instruction a, Instruction b, byte opCode ) : base(
             $"Instruction {a.Key} and {b.Key} share the same OpCode {opCode}",
             ErrorEventKeys.INSTR_DUPLIVATE_OP_CODE,
             false
            )
        {
        }

        #endregion

    }

}
