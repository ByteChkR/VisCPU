using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler.Special
{

    public class ReturnExpressionCompiler : HLExpressionCompiler < HLReturnOp >
    {
        #region Public

        public override ExpressionTarget ParseExpression( HLCompilation compilation, HLReturnOp expr )
        {
            if ( expr.Right != null )
            {
                ExpressionTarget pt = compilation.Parse(
                                                      expr.Right
                                                  ).
                                                  MakeAddress( compilation );

                compilation.EmitterResult.Emit( $"PUSH", pt.ResultAddress );

                compilation.ReleaseTempVar( pt.ResultAddress );
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
