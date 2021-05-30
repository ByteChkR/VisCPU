﻿using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Math.Bitwise
{

    public class BitwiseAndExpressionCompiler : HlExpressionCompiler < HlBinaryOp >
    {

        protected override bool NeedsOutput => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
            HlBinaryOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget target = compilation.Parse( expr.Left );

            ExpressionTarget rTargetVal = compilation.Parse(
                                                            expr.Right
                                                           );
            ExpressionTarget rTarget = rTargetVal.
                MakeAddress(compilation);

            if ( target.ResultAddress == outputTarget.ResultAddress )
            {
                compilation.EmitterResult.Emit(
                                               $"AND",
                                               target.ResultAddress,
                                               rTarget.ResultAddress
                                              );

                compilation.ReleaseTempVar(rTarget.ResultAddress);
                compilation.ReleaseTempVar(rTargetVal.ResultAddress);

                return target;
            }

            compilation.EmitterResult.Emit(
                                           $"AND",
                                           target.ResultAddress,
                                           rTarget.ResultAddress,
                                           outputTarget.ResultAddress
                                          );

            compilation.ReleaseTempVar( rTarget.ResultAddress );
            compilation.ReleaseTempVar( target.ResultAddress );
            compilation.ReleaseTempVar(rTargetVal.ResultAddress);

            return outputTarget;
        }

        #endregion

    }

}
