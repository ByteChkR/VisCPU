using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Math
{

    public class AddExpressionCompiler : HLExpressionCompiler < HLBinaryOp >
    {

        protected override bool NeedsOutput => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLBinaryOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget target = compilation.Parse( expr.Left, outputTarget );
            string tmp = compilation.GetTempVar();

            ExpressionTarget rTarget = compilation.Parse(
                                                         expr.Right,
                                                         new ExpressionTarget( tmp, true )
                                                        );

            if ( target.ResultAddress == outputTarget.ResultAddress )
            {
                compilation.ProgramCode.Add(
                                            $"ADD {target.ResultAddress} {rTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );

                compilation.ReleaseTempVar( tmp );

                return target;
            }

            if ( rTarget.ResultAddress == outputTarget.ResultAddress )
            {
                compilation.ProgramCode.Add(
                                            $"ADD {rTarget.ResultAddress} {target.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );

                return rTarget; //should never happen?
            }

            compilation.ProgramCode.Add(
                                        $"ADD {target.ResultAddress} {rTarget.ResultAddress} {outputTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                       );

            compilation.ReleaseTempVar( tmp );
            compilation.ReleaseTempVar( target.ResultAddress ); //??

            return outputTarget;
        }

        #endregion

    }

}
