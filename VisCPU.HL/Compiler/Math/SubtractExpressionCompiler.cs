using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Math
{
    public class SubtractExpressionCompiler : HLExpressionCompiler<HLBinaryOp>
    {
        protected override bool NeedsOutput => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLBinaryOp expr,
            ExpressionTarget outputTarget)
        {
            ExpressionTarget target = compilation.Parse(expr.Left, outputTarget);
            string rtName = compilation.GetTempVar();

            ExpressionTarget rTarget = compilation.Parse(
                expr.Right,
                new ExpressionTarget(rtName, true, compilation.TypeSystem.GetType("var"))
            );

            if (target.ResultAddress == outputTarget.ResultAddress)
            {
                compilation.ProgramCode.Add(
                    $"SUB {target.ResultAddress} {rTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                );

                compilation.ReleaseTempVar(rtName);
            }
            else if (rTarget.ResultAddress == outputTarget.ResultAddress)
            {
                compilation.ProgramCode.Add(
                    $"SUB {rTarget.ResultAddress} {target.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                );

                return rTarget;
            }
            else
            {
                compilation.ProgramCode.Add(
                    $"SUB {target.ResultAddress} {rTarget.ResultAddress} {outputTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                );

                compilation.ReleaseTempVar(rtName);
                compilation.ReleaseTempVar(target.ResultAddress);

                return outputTarget;
            }

            return target;
        }

        #endregion
    }
}