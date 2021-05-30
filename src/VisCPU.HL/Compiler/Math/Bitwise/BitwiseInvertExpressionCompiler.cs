using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Math.Bitwise
{

    public class BitwiseInvertExpressionCompiler : HlExpressionCompiler < HlUnaryOp >
    {

        protected override bool AllImplementations => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
            HlUnaryOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget targetVal = compilation.Parse(
                                                            expr.Left
                                                           );
            ExpressionTarget target = targetVal.
                MakeAddress(compilation);

            string tmp = compilation.GetTempVar( ~( uint ) 0 );
            compilation.EmitterResult.Emit( $"XOR", target.ResultAddress, tmp );

            ExpressionTarget ret = target.CopyIfNotNull( compilation, outputTarget );
            compilation.ReleaseTempVar(tmp);
            compilation.ReleaseTempVar( target.ResultAddress );

            return ret;
        }

        #endregion

    }

}
