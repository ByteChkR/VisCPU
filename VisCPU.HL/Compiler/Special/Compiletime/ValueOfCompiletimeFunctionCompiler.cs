using System.Linq;

using VisCPU.HL.Compiler.Memory;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler.Special.Compiletime
{

    public class ValueOfCompiletimeFunctionCompiler : ICompiletimeFunctionCompiler
    {

        public string FuncName => "val_of";

        #region Public

        public ExpressionTarget Compile( HLCompilation compilation, HLInvocationOp expr )
        {
            ExpressionTarget t = compilation.Parse( expr.ParameterList.First() );

            ExpressionTarget ret = ReferenceExpressionCompiler.Emit(
                                                                    compilation,
                                                                    t,
                                                                    new ExpressionTarget(
                                                                         compilation.GetTempVar( 0 ),
                                                                         true,
                                                                         compilation.TypeSystem.GetType( "var" )
                                                                        )
                                                                   );

            compilation.ReleaseTempVar( t.ResultAddress );

            return ret;
        }

        #endregion

    }

}
