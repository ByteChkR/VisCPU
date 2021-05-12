using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Instructions.Emit.Events
{

    internal class InvalidInstructionArgumentCountEvent : ErrorEvent
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
