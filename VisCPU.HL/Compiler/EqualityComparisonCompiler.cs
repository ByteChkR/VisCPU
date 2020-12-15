using System;

using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler
{
    public class EqualityComparisonCompiler : HLExpressionCompiler<HLBinaryOp>
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

            if ( target.IsArray )
            {
                ExpressionTarget tmp = new ExpressionTarget( compilation.GetTempVar( "array_un_ref_temp" ), true );
                compilation.ProgramCode.Add($"DREF {target.ResultAddress} {tmp.ResultAddress} ; Dereference Array Pointer (Equality Comparison)");
                target = tmp;
            }
            
            //BNE target rTarget if_b0_fail
            //LOAD possibleTarget 0x1; True Value
            //.if_b0_fail
            
            string label = compilation.GetUniqueName("bexpr_eq");
            compilation.ProgramCode.Add($"BNE {target.ResultAddress} {rTarget.ResultAddress} {label}");
            compilation.ProgramCode.Add($"LOAD {outputTarget.ResultAddress} 1");
            compilation.ProgramCode.Add($".{label} linker:hide");
            return outputTarget;
        }

    }
}