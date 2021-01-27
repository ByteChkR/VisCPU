namespace VisCPU.HL.Compiler.Relational
{

    public class InEqualityExpressionCompiler : RelationalExpressionCompiler
    {
        protected override string InstructionKey => "BNE";

        public override uint StaticEvaluate( ExpressionTarget a, ExpressionTarget b )
        {
            return a.StaticParse() != b.StaticParse() ? 1U : 0U;
        }
    }

}
