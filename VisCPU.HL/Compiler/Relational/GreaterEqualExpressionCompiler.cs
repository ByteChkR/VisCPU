namespace VisCPU.HL.Compiler.Relational
{

    public class GreaterEqualExpressionCompiler : RelationalExpressionCompiler
    {
        protected override string InstructionKey => "BGE";
    }

}
