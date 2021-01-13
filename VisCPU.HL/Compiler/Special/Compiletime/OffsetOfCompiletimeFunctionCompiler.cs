using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Compiler.Special.Compiletime
{

    public class OffsetOfCompiletimeFunctionCompiler : ICompiletimeFunctionCompiler
    {

        public string FuncName => "offset_of";

        #region Public

        public ExpressionTarget Compile( HLCompilation compilation, HLInvocationOp expr )
        {
            if ( expr.ParameterList.Length != 2 )
            {
                EventManager < ErrorEvent >.SendEvent(
                                                      new FunctionArgumentMismatchEvent(
                                                           "Invalid Arguments. Expected offset_of(type, member)"
                                                          )
                                                     );
            }

            HLTypeDefinition type = compilation.TypeSystem.GetType( expr.ParameterList[0].ToString() );
            uint off = type.GetOffset( expr.ParameterList[1].ToString() );
            string v = compilation.GetTempVar( off );

            return new ExpressionTarget( v, true, compilation.TypeSystem.GetType( "var" ) );
        }

        #endregion

    }

}
