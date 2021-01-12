﻿using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Logic
{

    public class BoolAndExpressionCompiler : HLExpressionCompiler < HLBinaryOp >
    {

        protected override bool NeedsOutput => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLBinaryOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget target = compilation.Parse(
                                                        expr.Left
                                                       ).MakeAddress(compilation);

            ExpressionTarget rTarget = compilation.Parse(
                                                         expr.Right
                                                        ).MakeAddress(compilation);

            //BNE target rTarget if_b0_fail
            //LOAD possibleTarget 0x1; True Value
            //.if_b0_fail
            string label = compilation.GetUniqueName( "bexpr_and" );
            compilation.ProgramCode.Add( $"LOAD {outputTarget.ResultAddress} 0" );
            compilation.ProgramCode.Add( $"BEZ {target.ResultAddress} {label}" );
            compilation.ProgramCode.Add( $"BEZ {rTarget.ResultAddress} {label}" );
            compilation.ProgramCode.Add( $"LOAD {outputTarget.ResultAddress} 1" );
            compilation.ProgramCode.Add( $".{label} linker:hide" );
            compilation.ReleaseTempVar( rTarget.ResultAddress );
            compilation.ReleaseTempVar( target.ResultAddress );

            return outputTarget;
        }

        #endregion

    }

}
