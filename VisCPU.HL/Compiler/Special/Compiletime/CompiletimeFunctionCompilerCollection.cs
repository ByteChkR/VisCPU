using System.Collections.Generic;

using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler.Special
{

    public class CompiletimeFunctionCompilerCollection
    {

        private Dictionary < string, ICompiletimeFunctionCompiler > compilers =
            new Dictionary < string, ICompiletimeFunctionCompiler >();

        public CompiletimeFunctionCompilerCollection()
        {
            AddCompiler(new OffsetOfCompiletimeFunctionCompiler());
            AddCompiler(new SizeOfCompiletimeFunctionCompiler());
            AddCompiler(new PointerOfCompiletimeFunctionCompiler());
            AddCompiler(new StringCompiletimeFunctionCompiler());
            AddCompiler(new StaticCastCompiletimeFunctionCompiler());
            AddCompiler(new OffsetOfCompiletimeFunctionCompiler());
        }

        public bool IsCompiletimeFunction( string key )
        {
            return compilers.ContainsKey(key);
        }

        public void AddCompiler(ICompiletimeFunctionCompiler comp)
        {
            compilers[comp.FuncName] = comp;
        }

        public ExpressionTarget Compile(string func, HLCompilation compilation, HLInvocationOp expr)
        {
            return compilers[func].Compile(compilation, expr);
        }

    }

}