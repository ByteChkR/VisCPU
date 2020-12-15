using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler
{
    public class LessComparisonCompiler : HLExpressionCompiler<HLBinaryOp>
    {

        protected override bool NeedsOutput => true;
        public override ExpressionTarget ParseExpression(
            HLCompilation compilation, HLBinaryOp expr, ExpressionTarget outputTarget)
        {
            ExpressionTarget target = compilation.Parse(
                                                        expr.Left
                                                       );
            ExpressionTarget rTarget = compilation.Parse(
                                                         expr.Right, new ExpressionTarget(compilation.GetTempVar(), true));

            //BNE target rTarget if_b0_fail
            //LOAD possibleTarget 0x1; True Value
            //.if_b0_fail
            string label = compilation.GetUniqueName("bexpr_lt");
            compilation.ProgramCode.Add($"BGE {target.ResultAddress} {rTarget.ResultAddress} {label}");
            compilation.ProgramCode.Add($"LOAD {outputTarget.ResultAddress} 1");
            compilation.ProgramCode.Add($".{label} linker:hide");
            return outputTarget;
        }

    }
}