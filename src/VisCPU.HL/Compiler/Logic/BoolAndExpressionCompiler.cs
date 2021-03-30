using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Logic
{

    public class BoolAndExpressionCompiler : HlExpressionCompiler < HlBinaryOp >
    {
        protected override bool NeedsOutput => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
            HlBinaryOp expr,
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
            string label = HlCompilation.GetUniqueName( "bexpr_and" );
            compilation.EmitterResult.Emit( $"LOAD", outputTarget.ResultAddress, "0" );
            compilation.EmitterResult.Emit( $"BEZ", target.ResultAddress, label );
            compilation.EmitterResult.Emit( $"BEZ", rTarget.ResultAddress, label );
            compilation.EmitterResult.Emit( $"LOAD", outputTarget.ResultAddress, "1" );
            compilation.EmitterResult.Store( $".{label} linker:hide" );
            compilation.ReleaseTempVar( rTarget.ResultAddress );
            compilation.ReleaseTempVar( target.ResultAddress );

            return outputTarget;
        }

        #endregion
    }

}
