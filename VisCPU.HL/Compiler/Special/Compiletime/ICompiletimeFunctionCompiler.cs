using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler.Special
{

    public interface ICompiletimeFunctionCompiler
    {

        string FuncName { get; }

        ExpressionTarget Compile( HLCompilation compilation, HLInvocationOp expr );

    }

}