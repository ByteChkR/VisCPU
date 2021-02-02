using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Compiler.Events
{

    public class UnaryMinusExpressionInvalidEvent : WarningEvent
    {
        #region Public

        public UnaryMinusExpressionInvalidEvent( string message ) : base(
            message,
            WarningEventKeys.s_InvertInvalidEvent )
        {
        }

        #endregion
    }

}
