using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler
{
    public class ReturnExpressionCompiler : HLExpressionCompiler<HLReturnOp>
    {

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation, HLReturnOp expr )
        {
            if (expr.Right != null)
            {
                ExpressionTarget pt = compilation.Parse(
                                                        expr.Right
                                                       ).MakeAddress(compilation);
                compilation.ProgramCode.Add($"PUSH {pt.ResultAddress}");
            }

            compilation.ProgramCode.Add("RET");

            return new ExpressionTarget();
        }

    }
}