using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Events
{

    public class ExpressionCompilerNotFoundEvent : ErrorEvent
    {

        #region Public

        public ExpressionCompilerNotFoundEvent( HLExpression expr ) : base(
                                                                           $"No Compiler found for expression: ({expr.Type}) '{expr}'",
                                                                           ErrorEventKeys.HL_COMPILER_NOT_FOUND,
                                                                           false
                                                                          )
        {
        }

        #endregion

    }

}
