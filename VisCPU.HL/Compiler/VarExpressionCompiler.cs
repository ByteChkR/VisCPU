using System;

using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.Utility.Events;

namespace VisCPU.HL.Compiler
{
    public class VarExpressionCompiler : HLExpressionCompiler<HLVarOperand>
    {

        protected override bool AllImplementations => true;

        public override ExpressionTarget ParseExpression(HLCompilation compilation, HLVarOperand expr)
        {
            return ParseExpression(compilation, expr, new ExpressionTarget());
        }

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation, HLVarOperand expr, ExpressionTarget outputTarget)
        {

            if (compilation.ConstValTypes.ContainsKey(expr.Value.ToString()))
            {
                return new ExpressionTarget(expr.Value.ToString(), true).CopyIfNotNull(compilation, outputTarget);
            }


            string varAddr;
            if (compilation.ContainsVariable(expr.Value.ToString()))
            {
                VariableData v = compilation.GetVariable(expr.Value.ToString());
                varAddr = v.GetFinalName();
                return new ExpressionTarget(varAddr, true, v.Size != 1).CopyIfNotNull(compilation, outputTarget);
            }
            else if (compilation.FunctionMap.ContainsKey(expr.Value.ToString()))
            {
                varAddr = expr.Value.ToString();
                return new ExpressionTarget(varAddr, true);
            }
            else
            {
                EventManager <ErrorEvent>.SendEvent(new HLVariableNotFoundEvent(expr.Value.ToString(), false));

                return new ExpressionTarget();

                //varAddr = compilation.GetTempVar();
            }
        }

    }
}