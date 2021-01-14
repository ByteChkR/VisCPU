namespace VisCPU.HL.Compiler.Math.Full
{

    public class DivExpressionCompiler : MathExpressionCompiler
    {

        protected override string InstructionKey => "DIV";
        protected override ExpressionTarget ComputeStatic(HLCompilation compilation, ExpressionTarget left, ExpressionTarget right)
        {
            return new ExpressionTarget($"{left.StaticParse() / right.StaticParse()}", false, left.TypeDefinition);
        }
    }

}
