using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Instructions.Emit.Events
{

    internal class InvalidArgumentCountEvent : ErrorEvent
    {
        #region Public

        public InvalidArgumentCountEvent( string instr, int argCount ) : base(
            $"Too many arguments for Instruction '{instr}' argument count: {argCount}",
            ErrorEventKeys.s_AsmGenTooManyArgs,
            false
        )
        {
        }

        #endregion
    }

}
