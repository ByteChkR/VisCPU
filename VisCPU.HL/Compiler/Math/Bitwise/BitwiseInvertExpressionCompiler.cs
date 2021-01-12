using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Math.Bitwise
{

    public class BitwiseInvertExpressionCompiler : HLExpressionCompiler < HLUnaryOp >
    {

        protected override bool AllImplementations => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLUnaryOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget target = compilation.Parse(
                                                        expr.Left
                                                       ).MakeAddress(compilation);

            string tmp = compilation.GetTempVar( ~( uint ) 0 );
            compilation.ProgramCode.Add( $"XOR {target} {tmp}" );

            return target.CopyIfNotNull( compilation, outputTarget, true );
        }

        #endregion

    }

}
