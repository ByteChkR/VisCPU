using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Compiler.Special.Compiletime
{

    public class HaltCompiletimeFunctionCompiler : ICompiletimeFunctionCompiler
    {
        public string FuncName => "halt";

        #region Public

        public ExpressionTarget Compile( HlCompilation compilation, HlInvocationOp expr )
        {
            if ( expr.ParameterList.Length != 0 )
            {
                EventManager < ErrorEvent >.SendEvent(
                    new FunctionArgumentMismatchEvent(
                        "Invalid Arguments. Expected halt()"
                    )
                );
            }

            compilation.EmitterResult.Emit( "HLT" );

            return default;
        }

        #endregion
    }

}
