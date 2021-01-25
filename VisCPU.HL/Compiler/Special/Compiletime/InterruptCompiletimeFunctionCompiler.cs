using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Compiler.Special.Compiletime
{

    public class InterruptCompiletimeFunctionCompiler : ICompiletimeFunctionCompiler
    {
        public string FuncName => "interrupt";

        #region Public

        public ExpressionTarget Compile( HlCompilation compilation, HlInvocationOp expr )
        {
            if ( expr.ParameterList.Length != 1 )
            {
                EventManager < ErrorEvent >.SendEvent(
                    new FunctionArgumentMismatchEvent(
                        "Invalid Arguments. Expected interrupt(code)"
                    )
                );
            }

            compilation.EmitterResult.Emit( "INT", expr.ParameterList[0].ToString() );

            return default;
        }

        #endregion
    }

}
