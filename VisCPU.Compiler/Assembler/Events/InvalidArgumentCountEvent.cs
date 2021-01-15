using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Compiler.Assembler.Events
{

    public class InvalidArgumentCountEvent : ErrorEvent
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
