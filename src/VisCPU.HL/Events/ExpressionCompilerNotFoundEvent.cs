using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

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
