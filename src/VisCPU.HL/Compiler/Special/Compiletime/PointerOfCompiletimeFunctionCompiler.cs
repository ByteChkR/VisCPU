using System.Linq;
using VisCPU.HL.Compiler.Memory;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Special.Compiletime
{

    public class PointerOfCompiletimeFunctionCompiler : ICompiletimeFunctionCompiler
    {
        public string FuncName => "ptr_of";

        #region Public

        public ExpressionTarget Compile( HlCompilation compilation, HlInvocationOp expr )
        {
            ExpressionTarget et = compilation.Parse( expr.ParameterList.First() );

            ExpressionTarget ret = ReferenceExpressionCompiler.Emit(
                compilation,
                et,
                new ExpressionTarget(
                    compilation.GetTempVar( 0 ),
                    true,
                    compilation.TypeSystem.GetType(
                        compilation.Root,
                        HLBaseTypeNames.s_UintTypeName
                    )
                )
            );

            compilation.ReleaseTempVar( et.ResultAddress );

            return ret;
        }

        #endregion
    }

}
