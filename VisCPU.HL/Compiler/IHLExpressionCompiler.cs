using VisCPU.HL.Parser.Tokens.Expressions;

namespace VisCPU.HL.Compiler
{

    internal interface IHlExpressionCompiler
    {

        ExpressionTarget Parse( HLCompilation compilation, HLExpression expr, ExpressionTarget outputTarget );

    }

}
