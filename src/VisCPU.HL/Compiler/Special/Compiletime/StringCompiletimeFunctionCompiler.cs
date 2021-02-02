using System.Linq;
using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Special.Compiletime
{

    public class StringCompiletimeFunctionCompiler : ICompiletimeFunctionCompiler
    {
        public string FuncName => "string";

        #region Public

        public ExpressionTarget Compile( HlCompilation compilation, HlInvocationOp expr )
        {
            if ( expr.ParameterList.Length != 2 )
            {
                EventManager < ErrorEvent >.SendEvent(
                    new FunctionArgumentMismatchEvent(
                        "Invalid Arguments. Expected string(varname, string value)"
                    )
                );
            }

            string varName = expr.ParameterList[0].ToString();

            string content = expr.ParameterList[1].
                                  GetChildren().
                                  Select( x => x.ToString() ).
                                  Aggregate( ( input, elem ) => input + ' ' + elem );

            compilation.CreateVariable(
                varName,
                content,
                compilation.TypeSystem.GetType( HLBaseTypeNames.s_UintTypeName ),
                false );

            return new ExpressionTarget(
                compilation.GetFinalName( varName ),
                true,
                compilation.TypeSystem.GetType( HLBaseTypeNames.s_UintTypeName )
            );
        }

        #endregion
    }

}
