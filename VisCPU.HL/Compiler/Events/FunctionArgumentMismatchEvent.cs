using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Compiler.Events
{

    public class FunctionArgumentMismatchEvent : ErrorEvent
    {

        #region Public

        public FunctionArgumentMismatchEvent( string errMessage ) : base(
                                                                         errMessage,
                                                                         ErrorEventKeys.HL_FUNCTION_ARGUMENT_MISMATCH,
                                                                         false
                                                                        )
        {
        }

        #endregion

    }

}
