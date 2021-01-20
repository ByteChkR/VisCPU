using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Math.Atomic
{

    public class DecrementExpressionCompiler : HlExpressionCompiler < HlUnaryOp >
    {
        #region Public

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlUnaryOp expr )
        {
            ExpressionTarget target = compilation.Parse( expr.Left );

            compilation.EmitterResult.Emit(
                $"DEC",
                target.ResultAddress
            );

            return target;
        }

        #endregion
    }

}
