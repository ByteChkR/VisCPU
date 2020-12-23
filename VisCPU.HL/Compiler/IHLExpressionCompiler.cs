using VisCPU.HL.Parser.Tokens.Expressions;

namespace VisCPU.HL.Compiler
{
    internal interface IHLExpressionCompiler
    {
        ExpressionTarget Parse(HLCompilation compilation, HLExpression expr, ExpressionTarget outputTarget);
    }
}