using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler
{
    public class ReturnExpressionCompiler : HLExpressionCompiler<HLReturnOp>
    {
        #region Public

        public override ExpressionTarget ParseExpression(HLCompilation compilation, HLReturnOp expr)
        {
            if (expr.Right != null)
            {
                ExpressionTarget pt = compilation.Parse(
                    expr.Right
                ).MakeAddress(compilation);

                compilation.ProgramCode.Add($"PUSH {pt.ResultAddress}");
            }
            else
            {
                string v = compilation.GetTempVar();
                compilation.ProgramCode.Add($"LOAD {v} 0");
                compilation.ProgramCode.Add($"PUSH {v}");
            }

            compilation.ProgramCode.Add("RET");

            return new ExpressionTarget();
        }

        #endregion
    }
}