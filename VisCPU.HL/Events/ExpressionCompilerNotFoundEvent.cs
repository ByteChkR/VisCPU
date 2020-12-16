using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.Utility.Events;

namespace VisCPU.HL
{

    public class ExpressionCompilerNotFoundEvent : ErrorEvent
    {

        private const string EVENT_KEY = "hl-compiler-not-found";

        public ExpressionCompilerNotFoundEvent(HLExpression expr) : base(
                                                                         $"No Compiler found for expression: ({expr.Type}) '{expr}'",
                                                                         EVENT_KEY,
                                                                         false
                                                                        )
        {
        }

    }

}