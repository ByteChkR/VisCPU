using System.Linq;

using VisCPU.HL.Compiler.Memory;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Special.Compiletime
{

    public class ValueOfCompiletimeFunctionCompiler : ICompiletimeFunctionCompiler
    {

        public string FuncName => "val_of";

        #region Public

        public ExpressionTarget Compile( HlCompilation compilation, HlInvocationOp expr )
        {
            ExpressionTarget t = compilation.Parse( expr.ParameterList.First() );

            ExpressionTarget ret = ReferenceExpressionCompiler.Emit(
                                                                    compilation,
                                                                    t,
                                                                    new ExpressionTarget(
                                                                         compilation.GetTempVar( 0 ),
                                                                         true,
                                                                         compilation.TypeSystem.GetType(
                                                                              HLBaseTypeNames.s_UintTypeName
                                                                             )
                                                                        )
                                                                   );

            compilation.ReleaseTempVar( t.ResultAddress );

            return ret;
        }

        #endregion

    }

}
