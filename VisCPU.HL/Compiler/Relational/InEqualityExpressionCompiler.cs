namespace VisCPU.HL.Compiler.Relational
{

    public class InEqualityExpressionCompiler : RelationalExpressionCompiler
    {
        protected override string InstructionKey => "BNE";
    }

}
