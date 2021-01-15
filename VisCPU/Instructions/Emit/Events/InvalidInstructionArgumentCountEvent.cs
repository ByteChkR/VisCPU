using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Instructions.Emit.Events
{

    public class InvalidInstructionArgumentCountEvent : ErrorEvent
    {

        #region Public

        public InvalidInstructionArgumentCountEvent( string instr, int argCount ) : base(
             $"The Instruction '{instr}' can not be used as it has {argCount} arguments while the max argument count is 3",
             ErrorEventKeys.s_AsmGenTooManyArgs,
             false
            )
        {
        }

        #endregion

    }

}
