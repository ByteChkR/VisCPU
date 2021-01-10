using System.Linq;

using VisCPU.HL.Compiler.Memory;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler.Special
{

    public class PointerOfCompiletimeFunctionCompiler: ICompiletimeFunctionCompiler
    {

        public string FuncName => "ptr_of";

        public ExpressionTarget Compile( HLCompilation compilation, HLInvocationOp expr )
        {
            ExpressionTarget et = compilation.Parse(expr.ParameterList.First());

            ExpressionTarget ret = ReferenceExpressionCompiler.Emit(
                                                                    compilation,
                                                                    et,
                                                                    new ExpressionTarget(
                                                                         compilation.GetTempVar(0),
                                                                         true,
                                                                         compilation.TypeSystem.GetType("var")
                                                                        )
                                                                   );

            compilation.ReleaseTempVar(et.ResultAddress);

            return ret;
        }

    }

}