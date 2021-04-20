﻿using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Math.Bitwise
{

    public class BitwiseOrExpressionCompiler : HlExpressionCompiler < HlBinaryOp >
    {

        protected override bool NeedsOutput => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
            HlBinaryOp expr,
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
                                               $"OR",
                                               target.ResultAddress,
                                               rTarget.ResultAddress
                                              );

                compilation.ReleaseTempVar( rTarget.ResultAddress );
            }
            else
            {
                compilation.EmitterResult.Emit(
                                               $"OR",
                                               target.ResultAddress,
                                               rTarget.ResultAddress,
                                               outputTarget.ResultAddress
                                              );

                compilation.ReleaseTempVar( rTarget.ResultAddress );
                compilation.ReleaseTempVar( target.ResultAddress );

                return outputTarget;
            }

            return target;
        }

        #endregion

    }

}
