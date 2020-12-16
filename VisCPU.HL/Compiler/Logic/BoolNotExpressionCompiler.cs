using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler
{
    public class BoolNotExpressionCompiler : HLExpressionCompiler<HLUnaryOp>
    {

        protected override bool AllImplementations => true;

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation, HLUnaryOp expr, ExpressionTarget outputTarget)
        {
            string tName = compilation.GetTempVar();
            ExpressionTarget target = compilation.Parse(
                                                        expr.Left,
                                                        new ExpressionTarget(tName, true)
                                                       );

            //BNE target rTarget if_b0_fail
            //LOAD possibleTarget 0x1; True Value
            //.if_b0_fail
            string label = compilation.GetUniqueName("bexpr_not");
            string labelF = compilation.GetUniqueName("bexpr_not_f");
            compilation.ProgramCode.Add($"BNZ {target.ResultAddress} {labelF}");
            compilation.ProgramCode.Add($"LOAD {outputTarget.ResultAddress} 1");
            compilation.ProgramCode.Add($"JMP {label}");
            compilation.ProgramCode.Add($".{labelF} linker:hide");
            compilation.ProgramCode.Add($"LOAD {outputTarget.ResultAddress} 0");
            compilation.ProgramCode.Add($".{label} linker:hide");
            compilation.ReleaseTempVar( tName );
            return outputTarget;
        }

    }
}