using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Bitwise
{
    public class BitwiseAndExpressionCompiler : HLExpressionCompiler<HLBinaryOp>
    {
        protected override bool NeedsOutput => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLBinaryOp expr,
            ExpressionTarget outputTarget)
        {
            ExpressionTarget target = compilation.Parse(expr.Left);
            string rtName = compilation.GetTempVar();

            ExpressionTarget rTarget = compilation.Parse(
                expr.Right,
                new ExpressionTarget(rtName, true, compilation.TypeSystem.GetType("var"))
            );

            if (target.ResultAddress == outputTarget.ResultAddress)
            {
                compilation.ProgramCode.Add(
                    $"AND {target.ResultAddress} {rTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                );

                compilation.ReleaseTempVar(rtName);

                return target;
            }

            if (rTarget.ResultAddress == outputTarget.ResultAddress)
            {
                compilation.ProgramCode.Add(
                    $"AND {rTarget.ResultAddress} {target.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                );

                return rTarget;
            }

            compilation.ProgramCode.Add(
                $"AND {target.ResultAddress} {rTarget.ResultAddress} {outputTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
            );

            compilation.ReleaseTempVar(rtName);
            compilation.ReleaseTempVar(target.ResultAddress);

            return outputTarget;
        }

        #endregion
    }
}