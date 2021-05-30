﻿using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Logic
{

    public class BoolOrExpressionCompiler : HlExpressionCompiler < HlBinaryOp >
    {

        protected override bool NeedsOutput => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
            HlBinaryOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget targetVal = compilation.Parse(
                                                           expr.Left
                                                          );
            ExpressionTarget target = targetVal.
                MakeAddress(compilation);

            ExpressionTarget rTargetVal = compilation.Parse(
                                                            expr.Right
                                                           );
            ExpressionTarget rTarget = rTargetVal.
                                                   MakeAddress(compilation);

            //BNE target rTarget if_b0_fail
            //LOAD possibleTarget 0x1; True Value
            //.if_b0_fail
            string label = HlCompilation.GetUniqueName( "bexpr_or" );
            string labelF = HlCompilation.GetUniqueName( "bexpr_or_f" );
            compilation.EmitterResult.Emit( $"BNZ", target.ResultAddress, label );
            compilation.EmitterResult.Emit( $"BEZ", rTarget.ResultAddress, labelF );
            compilation.EmitterResult.Store( $".{label} linker:hide" );
            compilation.EmitterResult.Emit( $"LOAD", outputTarget.ResultAddress, "1" );
            compilation.EmitterResult.Store( $".{labelF} linker:hide" );
            compilation.ReleaseTempVar(rTarget.ResultAddress);
            compilation.ReleaseTempVar(target.ResultAddress);

            compilation.ReleaseTempVar(rTargetVal.ResultAddress);
            compilation.ReleaseTempVar(targetVal.ResultAddress);

            return outputTarget;
        }

        #endregion

    }

}
