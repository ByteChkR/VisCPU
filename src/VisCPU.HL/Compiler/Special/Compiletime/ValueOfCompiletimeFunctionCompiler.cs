using System.Linq;

using VisCPU.HL.Compiler.Memory;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility.IO.Settings;
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
                                                                         SettingsManager.GetSettings<HlCompilerSettings>().OmitTempVarInit
                                                                             ? compilation.GetTempVar()
                                                                             : compilation.GetTempVar(0),
                                                                         true,
                                                                         compilation.TypeSystem.GetType(
                                                                              compilation.Root,
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
