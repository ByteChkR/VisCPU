using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Compiler.Events
{

    public class FunctionNotFoundEvent : ErrorEvent
    {

        #region Public

        public FunctionNotFoundEvent( string funcName ) : base(
                                                               $"Function '{funcName}' not found",
                                                               ErrorEventKeys.HL_FUNCTION_NOT_FOUND,
                                                               false
                                                              )
        {
        }

        #endregion

    }

}
