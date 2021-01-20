using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Math.Atomic
{

    public class DecrementExpressionCompiler : HLExpressionCompiler < HLUnaryOp >
    {
        #region Public

        public override ExpressionTarget ParseExpression( HLCompilation compilation, HLUnaryOp expr )
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
