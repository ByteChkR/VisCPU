using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Compiler.Events
{

    public class UnaryMinusExpressionInvalidEvent : WarningEvent
    {
        #region Public

        public UnaryMinusExpressionInvalidEvent( string message ) : base(
            message,
            WarningEventKeys.s_InvertInvalidEvent
        )
        {
        }

        #endregion
    }

}
