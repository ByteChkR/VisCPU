using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Events
{

    internal class ExpressionCompilerNotFoundEvent : ErrorEvent
    {
        #region Public

        public ExpressionCompilerNotFoundEvent( HlExpression expr ) : base(
            $"No Compiler found for expression: ({expr.Type}) '{expr}'",
            ErrorEventKeys.s_HlCompilerNotFound,
            false
        )
        {
        }

        #endregion
    }

}
