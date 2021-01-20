namespace VisCPU.HL.Compiler.Math.Full
{

    public class MulExpressionCompiler : MathExpressionCompiler
    {
        protected override string InstructionKey => "MUL";

        #region Protected

        protected override ExpressionTarget ComputeStatic(
            HLCompilation compilation,
            ExpressionTarget left,
            ExpressionTarget right )
        {
            return new ExpressionTarget( $"{left.StaticParse() * right.StaticParse()}", false, left.TypeDefinition );
        }

        #endregion
    }

}
