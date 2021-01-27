namespace VisCPU.HL.Compiler.Relational
{

    public class GreaterEqualExpressionCompiler : RelationalExpressionCompiler
    {
        protected override string InstructionKey => "BGE";

        public override uint StaticEvaluate( ExpressionTarget a, ExpressionTarget b )
        {
            return a.StaticParse() <= b.StaticParse() ? 1U : 0U;
        }
    }

}
