using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler
{
    public class ArrayAccessCompiler : HLExpressionCompiler<HLArrayAccessorOp>
    {

        protected override bool AllImplementations => true;

        public override ExpressionTarget ParseExpression(HLCompilation compilation, HLArrayAccessorOp expr, ExpressionTarget outputTarget)
        {
            ExpressionTarget tempPtr = new ExpressionTarget(compilation.GetTempVar(), true, true);

            ExpressionTarget tempPtrVar = compilation.Parse(expr.Left);
            ExpressionTarget pn = compilation.Parse(expr.ParameterList[0], new ExpressionTarget(compilation.GetTempVar(), true));


            if (tempPtrVar.IsArray)
            {
                compilation.ProgramCode.Add($"LOAD {tempPtr.ResultAddress} {tempPtrVar.ResultAddress}");
            }
            else
            {
                compilation.ProgramCode.Add($"COPY {tempPtrVar.ResultAddress} {tempPtr.ResultAddress}");
            }
            compilation.ProgramCode.Add($"ADD {tempPtr.ResultAddress} {pn.ResultAddress} ; Apply offset");

            if (outputTarget.ResultAddress != null)
            {
                compilation.ProgramCode.Add($"DREF {tempPtr.ResultAddress} {outputTarget.ResultAddress} ; Dereference Array Pointer");
                return outputTarget;
            }

            return tempPtr;
        }

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation, HLArrayAccessorOp expr)
        {
            return ParseExpression(compilation, expr, new ExpressionTarget());
        }

    }
}