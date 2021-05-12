using System.Collections.Generic;

using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler.Special.Compiletime
{

    public class CompiletimeFunctionCompilerCollection
    {

        private readonly Dictionary < string, ICompiletimeFunctionCompiler > m_Compilers =
            new Dictionary < string, ICompiletimeFunctionCompiler >();

        #region Public

        public CompiletimeFunctionCompilerCollection()
        {
            AddCompiler( new OffsetOfCompiletimeFunctionCompiler() );
            AddCompiler( new SizeOfCompiletimeFunctionCompiler() );
            AddCompiler( new PointerOfCompiletimeFunctionCompiler() );
            AddCompiler( new StringCompiletimeFunctionCompiler() );
            AddCompiler( new StaticCastCompiletimeFunctionCompiler() );
            AddCompiler( new InterruptCompiletimeFunctionCompiler() );
            AddCompiler( new InlineVASMEmitCompiletimeFunctionCompiler() );
            AddCompiler( new HaltCompiletimeFunctionCompiler() );
            AddCompiler( new ValueOfCompiletimeFunctionCompiler() );
        }

        public void AddCompiler( ICompiletimeFunctionCompiler comp )
        {
            m_Compilers[comp.FuncName] = comp;
        }

        public ExpressionTarget Compile( string func, HlCompilation compilation, HlInvocationOp expr )
        {
            return m_Compilers[func].Compile( compilation, expr );
        }

        public bool IsCompiletimeFunction( string key )
        {
            return m_Compilers.ContainsKey( key );
        }

        #endregion

    }

}
