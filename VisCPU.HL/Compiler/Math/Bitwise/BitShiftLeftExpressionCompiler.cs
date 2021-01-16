using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Math.Bitwise
{

    public class BitShiftLeftExpressionCompiler : HLExpressionCompiler < HLBinaryOp >
    {

        protected override bool NeedsOutput => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLBinaryOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget target = compilation.Parse( expr.Left );

            ExpressionTarget rTarget = compilation.Parse(
                                                         expr.Right
                                                        ).
                                                   MakeAddress( compilation );

            if ( target.ResultAddress == outputTarget.ResultAddress )
            {
                compilation.EmitterResult.Emit(
                                               $"SHL",
                                               target.ResultAddress,
                                               rTarget.ResultAddress
                                              );

                compilation.ReleaseTempVar( rTarget.ResultAddress );

                return target;
            }

            compilation.EmitterResult.Emit(
                                           $"SHL",
                                           target.ResultAddress,
                                           rTarget.ResultAddress,
                                           outputTarget.ResultAddress
                                          );

            compilation.ReleaseTempVar( rTarget.ResultAddress );
            compilation.ReleaseTempVar( target.ResultAddress );

            return outputTarget;
        }

        #endregion

    }

}
