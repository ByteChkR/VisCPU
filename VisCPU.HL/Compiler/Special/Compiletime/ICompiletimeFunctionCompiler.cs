using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler.Special.Compiletime
{

    public interface ICompiletimeFunctionCompiler
    {
        ExpressionTarget Compile( HLCompilation compilation, HLInvocationOp expr );

        string FuncName { get; }
    }

}
