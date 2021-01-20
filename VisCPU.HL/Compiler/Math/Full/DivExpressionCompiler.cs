namespace VisCPU.HL.Compiler.Math.Full
{

    public class DivExpressionCompiler : MathExpressionCompiler
    {
        protected override string InstructionKey => "DIV";

        #region Protected

        protected override ExpressionTarget ComputeStatic(
            HlCompilation compilation,
            ExpressionTarget left,
            ExpressionTarget right )
        {
            return new ExpressionTarget( $"{left.StaticParse() / right.StaticParse()}", false, left.TypeDefinition );
        }

        #endregion
    }

}
