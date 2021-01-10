using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Math.Bitwise
{

    public class BitwiseOrExpressionCompiler : HLExpressionCompiler < HLBinaryOp >
    {

        protected override bool NeedsOutput => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLBinaryOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget target = compilation.Parse( expr.Left );

            ExpressionTarget rTarget = compilation.Parse(
                                                         expr.Right,
                                                         new ExpressionTarget(
                                                                              compilation.GetTempVar( 0 ),
                                                                              true,
                                                                              compilation.TypeSystem.GetType( "var" )
                                                                             )
                                                        );

            if ( target.ResultAddress == outputTarget.ResultAddress )
            {
                compilation.ProgramCode.Add(
                                            $"OR {target.ResultAddress} {rTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );

                compilation.ReleaseTempVar( rTarget.ResultAddress );
            }
            else
            {
                compilation.ProgramCode.Add(
                                            $"OR {target.ResultAddress} {rTarget.ResultAddress} {outputTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
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
