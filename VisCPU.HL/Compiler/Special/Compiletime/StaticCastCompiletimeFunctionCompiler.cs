using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Compiler.Special.Compiletime
{

    public class StaticCastCompiletimeFunctionCompiler : ICompiletimeFunctionCompiler
    {
        public string FuncName => "static_cast";

        #region Public

        public ExpressionTarget Compile( HlCompilation compilation, HlInvocationOp expr )
        {
            if ( expr.ParameterList.Length != 2 )
            {
                EventManager < ErrorEvent >.SendEvent(
                    new FunctionArgumentMismatchEvent(
                        "Invalid Arguments. Expected static_cast(variable, type)"
                    )
                );
            }

            return compilation.Parse( expr.ParameterList[0] ).
                               Cast( compilation.TypeSystem.GetType( expr.ParameterList[1].ToString() ) );
        }

        #endregion
    }

}
