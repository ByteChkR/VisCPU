using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler
{
    public class BitwiseAndExpressionCompiler : HLExpressionCompiler<HLBinaryOp>
    {

        protected override bool NeedsOutput => true;

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation, HLBinaryOp expr, ExpressionTarget outputTarget)
        {
            ExpressionTarget target = compilation.Parse(expr.Left);
            string rtName = compilation.GetTempVar();
            ExpressionTarget rTarget = compilation.Parse(
                                                         expr.Right, new ExpressionTarget(rtName, true));

            if (target.ResultAddress == outputTarget.ResultAddress)
            {
                compilation.ProgramCode.Add(
                                            $"AND {target.ResultAddress} {rTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );

                compilation.ReleaseTempVar( rtName );
                return target;
            }
            else if (rTarget.ResultAddress == outputTarget.ResultAddress)
            {
                compilation.ProgramCode.Add(
                                            $"AND {rTarget.ResultAddress} {target.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );
                return rTarget;
            }
            else
            {
                compilation.ProgramCode.Add(
                                            $"AND {target.ResultAddress} {rTarget.ResultAddress} {outputTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );

                compilation.ReleaseTempVar( rtName );
                compilation.ReleaseTempVar( target.ResultAddress );
                return outputTarget;
            }

        }

    }
}