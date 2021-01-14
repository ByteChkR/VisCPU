namespace VisCPU.HL.Compiler.Math.Full
{

    public class SubExpressionCompiler : MathExpressionCompiler
    {

        protected override string InstructionKey => "SUB";
        protected override ExpressionTarget ComputeStatic(HLCompilation compilation, ExpressionTarget left, ExpressionTarget right)
        {
            return new ExpressionTarget($"{left.StaticParse() - right.StaticParse()}", false, left.TypeDefinition);
        }
    }

}
