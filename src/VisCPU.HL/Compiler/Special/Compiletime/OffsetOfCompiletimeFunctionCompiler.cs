using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Special.Compiletime
{

    public class OffsetOfCompiletimeFunctionCompiler : ICompiletimeFunctionCompiler
    {

        public string FuncName => "offset_of";

        #region Public

        public ExpressionTarget Compile( HlCompilation compilation, HlInvocationOp expr )
        {
            if ( expr.ParameterList.Length != 2 )
            {
                EventManager < ErrorEvent >.SendEvent(
                                                      new FunctionArgumentMismatchEvent(
                                                           "Invalid Arguments. Expected offset_of(type, member)"
                                                          )
                                                     );
            }

            HlTypeDefinition type = compilation.TypeSystem.GetType(compilation.Root, expr.ParameterList[0].ToString() );
            uint off = type.GetOffset( expr.ParameterList[1].ToString() );
            string v = compilation.GetTempVar( off );

            return new ExpressionTarget( v, true, compilation.TypeSystem.GetType(compilation.Root, HLBaseTypeNames.s_UintTypeName ) );
        }

        #endregion

    }

}
