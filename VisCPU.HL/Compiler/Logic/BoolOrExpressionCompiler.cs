using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Logic
{

    public class BoolOrExpressionCompiler : HLExpressionCompiler < HLBinaryOp >
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
                                                  ).
                                                  MakeAddress( compilation );

            ExpressionTarget rTarget = compilation.Parse(
                                                       expr.Right
                                                   ).
                                                   MakeAddress( compilation );

            //BNE target rTarget if_b0_fail
            //LOAD possibleTarget 0x1; True Value
            //.if_b0_fail
            string label = HLCompilation.GetUniqueName( "bexpr_or" );
            string labelF = HLCompilation.GetUniqueName( "bexpr_or_f" );
            compilation.EmitterResult.Emit( $"BNZ", target.ResultAddress, label );
            compilation.EmitterResult.Emit( $"BEZ", rTarget.ResultAddress, labelF );
            compilation.EmitterResult.Store( $".{label} linker:hide" );
            compilation.EmitterResult.Emit( $"LOAD", outputTarget.ResultAddress, "1" );
            compilation.EmitterResult.Store( $".{labelF} linker:hide" );
            compilation.ReleaseTempVar( rTarget.ResultAddress );
            compilation.ReleaseTempVar( target.ResultAddress );

            return outputTarget;
        }

        #endregion
    }

}
