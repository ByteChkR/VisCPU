﻿using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler
{
    public class GreaterEqualComparisonCompiler : HLExpressionCompiler<HLBinaryOp>
    {

        protected override bool NeedsOutput => true;
        public override ExpressionTarget ParseExpression(
            HLCompilation compilation, HLBinaryOp expr, ExpressionTarget outputTarget)
        {
            ExpressionTarget target = compilation.Parse(
                                                        expr.Left
                                                       ).MakeAddress(compilation);
            string rtName = compilation.GetTempVar();
            ExpressionTarget rTarget = compilation.Parse(
                                                         expr.Right, new ExpressionTarget(rtName, true));

            //BNE target rTarget if_b0_fail
            //LOAD possibleTarget 0x1; True Value
            //.if_b0_fail
            string label = compilation.GetUniqueName("bexpr_ge");
            compilation.ProgramCode.Add($"BLT {target.ResultAddress} {rTarget.ResultAddress} {label}");
            compilation.ProgramCode.Add($"LOAD {outputTarget.ResultAddress} 1");
            compilation.ProgramCode.Add($".{label} linker:hide");
            compilation.ReleaseTempVar( rtName );
            compilation.ReleaseTempVar( target.ResultAddress );
            return outputTarget;
        }

    }
}