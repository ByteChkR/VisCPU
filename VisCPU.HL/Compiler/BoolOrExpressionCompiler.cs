using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler
{
    public class BoolOrExpressionCompiler : HLExpressionCompiler<HLBinaryOp>
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
            string label = compilation.GetUniqueName("bexpr_or");
            string labelF = compilation.GetUniqueName("bexpr_or_f");
            compilation.ProgramCode.Add($"BNZ {target.ResultAddress} {label}");
            compilation.ProgramCode.Add($"BEZ {rTarget.ResultAddress} {labelF}");
            compilation.ProgramCode.Add($".{label} linker:hide");
            compilation.ProgramCode.Add($"LOAD {outputTarget.ResultAddress} 1");
            compilation.ProgramCode.Add($".{labelF} linker:hide");
            return outputTarget;
        }

    }
}