using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler
{
    public class AddExpressionCompiler : HLExpressionCompiler<HLBinaryOp>
    {

        protected override bool NeedsOutput => true;

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation, HLBinaryOp expr, ExpressionTarget outputTarget)
        {
            ExpressionTarget target = compilation.Parse(expr.Left, outputTarget);
            string tmp = compilation.GetTempVar();
            ExpressionTarget rTarget = compilation.Parse(
                                                         expr.Right, new ExpressionTarget(tmp, true));

            if (target.ResultAddress == outputTarget.ResultAddress)
            {
                compilation.ProgramCode.Add(
                                            $"ADD {target.ResultAddress} {rTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );

                compilation.ReleaseTempVar( tmp );
                return target;
            }
            else if (rTarget.ResultAddress == outputTarget.ResultAddress)
            {
                compilation.ProgramCode.Add(
                                            $"ADD {rTarget.ResultAddress} {target.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );
                return rTarget; //should never happen?
            }
            else
            {
                compilation.ProgramCode.Add(
                                            $"ADD {target.ResultAddress} {rTarget.ResultAddress} {outputTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );

                compilation.ReleaseTempVar( tmp );
                compilation.ReleaseTempVar( target.ResultAddress ); //??
                return outputTarget;
            }

        }

    }
}