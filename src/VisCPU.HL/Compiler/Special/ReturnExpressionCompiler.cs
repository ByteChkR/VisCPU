using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler.Special
{

    public class ReturnExpressionCompiler : HlExpressionCompiler < HlReturnOp >
    {

        #region Public

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlReturnOp expr )
        {
            if ( expr.Right != null )
            {
                ExpressionTarget ptVal = compilation.Parse(
                                                           expr.Right
                                                          );
                ExpressionTarget pt = ptVal.
                                                  MakeAddress( compilation );

                compilation.EmitterResult.Emit( $"PUSH", pt.ResultAddress );

                compilation.ReleaseTempVar(pt.ResultAddress);
                compilation.ReleaseTempVar(ptVal.ResultAddress);
            }
            else
            {
                string v = compilation.GetTempVar( 0 );
                compilation.EmitterResult.Emit( $"PUSH", v );
                compilation.ReleaseTempVar( v );
            }

            compilation.EmitterResult.Emit( "RET" );

            return new ExpressionTarget();
        }

        #endregion

    }

}
