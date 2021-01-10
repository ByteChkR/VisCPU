using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Instructions.Emit.Events
{

    public class InvalidArgumentCountEvent : ErrorEvent
    {

        #region Public

        public InvalidArgumentCountEvent( string instr, int argCount ) : base(
                                                                              $"Too many arguments for Instruction '{instr}' argument count: {argCount}",
                                                                              ErrorEventKeys.ASM_GEN_TOO_MANY_ARGS,
                                                                              false
                                                                             )
        {
        }

        #endregion

    }

}
