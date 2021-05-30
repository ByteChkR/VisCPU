using System.Linq;

using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Compiler.Special.Compiletime
{

    public class InlineVASMEmitCompiletimeFunctionCompiler : ICompiletimeFunctionCompiler
    {

        public string FuncName => "inl_vasm";

        #region Public

        public ExpressionTarget Compile( HlCompilation compilation, HlInvocationOp expr )
        {
            if ( expr.ParameterList.Length > 5 )
            {
                EventManager < ErrorEvent >.SendEvent(
                                                      new FunctionArgumentMismatchEvent(
                                                           "Invalid Arguments. Expected:\n inl_vasm(Instruction)\n inl_vasm(Instruction, Arg0)\n inl_vasm(Instruction, Arg0, Arg1)\n inl_vasm(Instruction, Arg0, Arg1, Arg2)\n inl_vasm(Instruction, Arg0, Arg1, Arg2, Arg3)"
                                                          )
                                                     );
            }

            string[] args = expr.ParameterList.Skip( 1 ).Select( x => x.ToString() ).ToArray();
            compilation.EmitterResult.Emit( expr.ParameterList[0].ToString(), args );

            return default;
        }

        #endregion

    }

}