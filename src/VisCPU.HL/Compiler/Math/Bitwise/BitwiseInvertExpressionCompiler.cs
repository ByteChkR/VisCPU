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
            ExpressionTarget target = compilation.Parse(
                                                      expr.Left
                                                  ).
                                                  MakeAddress( compilation );

            string tmp = compilation.GetTempVar( ~( uint ) 0 );
            compilation.EmitterResult.Emit( $"XOR", target.ResultAddress, tmp );

            return target.CopyIfNotNull( compilation, outputTarget );
        }

        #endregion
    }

}
