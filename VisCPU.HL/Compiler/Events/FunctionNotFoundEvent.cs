using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Compiler.Events
{

    public class FunctionNotFoundEvent : ErrorEvent
    {

        #region Public

        public FunctionNotFoundEvent( string funcName ) : base(
                                                               $"Function '{funcName}' not found",
                                                               ErrorEventKeys.s_HlFunctionNotFound,
                                                               false
                                                              )
        {
        }

        #endregion

    }

}
