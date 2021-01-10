using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Events;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Compiler.Special
{

    public class SizeOfCompiletimeFunctionCompiler : ICompiletimeFunctionCompiler
    {

        public string FuncName => "size_of";

        public ExpressionTarget Compile(HLCompilation compilation, HLInvocationOp expr)
        {
            if (expr.ParameterList.Length != 1)
            {
                EventManager<ErrorEvent>.SendEvent(
                                                   new FunctionArgumentMismatchEvent(
                                                        "Invalid Arguments. Expected size_of(variable)"
                                                       )
                                                  );
            }

            if (compilation.ContainsVariable(expr.ParameterList[0].ToString()))
            {
                string v = compilation.GetTempVar(
                                                  compilation.GetVariable(expr.ParameterList[0].ToString()).Size
                                                 );

                return new ExpressionTarget(v, true, compilation.TypeSystem.GetType("var"));
            }

            if (compilation.TypeSystem.HasType(expr.ParameterList[0].ToString()))
            {
                string v = compilation.GetTempVar(
                                                  compilation.TypeSystem.
                                                              GetType(expr.ParameterList[0].ToString()).
                                                              GetSize()
                                                 );

                return new ExpressionTarget(v, true, compilation.TypeSystem.GetType("var"));
            }

            EventManager<ErrorEvent>.SendEvent(
                                               new HLVariableNotFoundEvent(
                                                                           expr.ParameterList[0].ToString(),
                                                                           false
                                                                          )
                                              );

            return new ExpressionTarget();
        }

    }

}