using VisCPU.HL.Parser.Tokens.Expressions;

namespace VisCPU.HL.Compiler
{

    internal interface IHlExpressionCompiler
    {
        ExpressionTarget Parse( HlCompilation compilation, HlExpression expr, ExpressionTarget outputTarget );
    }

}
