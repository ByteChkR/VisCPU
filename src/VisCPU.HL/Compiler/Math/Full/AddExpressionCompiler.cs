namespace VisCPU.HL.Compiler.Math.Full
{

    public class AddExpressionCompiler : MathExpressionCompiler
    {
        protected override string InstructionKey => "ADD";

        #region Protected

        protected override ExpressionTarget ComputeStatic(
            HlCompilation compilation,
            ExpressionTarget left,
            ExpressionTarget right )
        {
            return new ExpressionTarget( $"{left.StaticParse() + right.StaticParse()}", false, left.TypeDefinition );
        }

        #endregion
    }

}
