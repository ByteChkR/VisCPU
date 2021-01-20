namespace VisCPU.HL.Compiler.Relational
{

    public class LessEqualExpressionCompiler : RelationalExpressionCompiler
    {
        protected override string InstructionKey => "BLE";
    }

}
