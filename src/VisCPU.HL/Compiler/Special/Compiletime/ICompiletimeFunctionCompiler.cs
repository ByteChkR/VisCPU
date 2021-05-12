using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler.Special.Compiletime
{

    public interface ICompiletimeFunctionCompiler
    {

        ExpressionTarget Compile( HlCompilation compilation, HlInvocationOp expr );

        string FuncName { get; }

    }

}
