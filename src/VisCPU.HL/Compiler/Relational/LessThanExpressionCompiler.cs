namespace VisCPU.HL.Compiler.Relational
{

    public class LessThanExpressionCompiler : RelationalExpressionCompiler
    {
        protected override string InstructionKey => "BLT";

        #region Public

        public override uint StaticEvaluate( ExpressionTarget a, ExpressionTarget b )
        {
            return a.StaticParse() < b.StaticParse() ? 1U : 0U;
        }

        #endregion
    }

}
