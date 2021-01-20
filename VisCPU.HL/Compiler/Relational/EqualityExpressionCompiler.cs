namespace VisCPU.HL.Compiler.Relational
{

    public class EqualityExpressionCompiler : RelationalExpressionCompiler
    {
        protected override string InstructionKey => "BEQ";
    }

}
