using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler
{
    public class BitwiseOrExpressionCompiler : HLExpressionCompiler<HLBinaryOp>
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
                                            $"OR {target.ResultAddress} {rTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );

                compilation.ReleaseTempVar( rtName );
            }
            else if (rTarget.ResultAddress == outputTarget.ResultAddress)
            {
                compilation.ProgramCode.Add(
                                            $"OR {rTarget.ResultAddress} {target.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );
                return rTarget;
            }
            else
            {
                compilation.ProgramCode.Add(
                                            $"OR {target.ResultAddress} {rTarget.ResultAddress} {outputTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );

                compilation.ReleaseTempVar( rtName );
                compilation.ReleaseTempVar( target.ResultAddress );
                return outputTarget;
            }

            return target;
        }

    }
}