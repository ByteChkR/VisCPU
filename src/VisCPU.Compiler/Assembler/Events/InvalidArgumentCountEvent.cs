using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Compiler.Assembler.Events
{

    internal class InvalidArgumentCountEvent : ErrorEvent
    {

        #region Public

        public InvalidArgumentCountEvent( int line ) : base(
                                                            $"Too many arguments in line: '{line}'",
                                                            ErrorEventKeys.s_AsmGenTooManyArgs,
                                                            false
                                                           )
        {
        }

        #endregion

    }

}
