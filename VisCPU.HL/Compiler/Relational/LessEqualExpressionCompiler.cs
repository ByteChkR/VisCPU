namespace VisCPU.HL.Compiler.Relational
{

    public class LessEqualExpressionCompiler : RelationalExpressionCompiler
    {
        protected override string InstructionKey => "BLE";

        public override uint StaticEvaluate( ExpressionTarget a, ExpressionTarget b )
        {
            return a.StaticParse() <= b.StaticParse() ? 1U : 0U;
        }
    }

}
