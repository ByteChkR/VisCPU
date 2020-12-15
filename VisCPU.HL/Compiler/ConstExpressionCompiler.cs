using VisCPU.HL.Parser.Tokens.Expressions.Operands;

namespace VisCPU.HL.Compiler
{
    public class ConstExpressionCompiler : HLExpressionCompiler<HLValueOperand>
    {

        protected override bool AllImplementations => true;


        public override ExpressionTarget ParseExpression(HLCompilation compilation, HLValueOperand expr)
        {
            ExpressionTarget tmp = new ExpressionTarget(expr.Value.ToString(), false);
            return tmp;
        }

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation, HLValueOperand expr, ExpressionTarget outputTarget)
        {
            compilation.ProgramCode.Add($"LOAD {outputTarget.ResultAddress} {expr.Value}");
            return outputTarget;
        }

    }
}