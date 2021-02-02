namespace VisCPU.HL.Compiler.Relational
{

    public class EqualityExpressionCompiler : RelationalExpressionCompiler
    {

        protected override string InstructionKey => "BEQ";

        #region Public

        public override uint StaticEvaluate( ExpressionTarget a, ExpressionTarget b )
        {
            return a.StaticParse() == b.StaticParse() ? 1U : 0U;
        }

        #endregion

    }

}
