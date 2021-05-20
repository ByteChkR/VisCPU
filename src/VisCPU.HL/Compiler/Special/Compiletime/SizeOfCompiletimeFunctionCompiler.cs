using System;

using VisCPU.HL.Compiler.Events;
using VisCPU.HL.DataTypes;
using VisCPU.HL.Events;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Special.Compiletime
{

    public class SizeOfCompiletimeFunctionCompiler : ICompiletimeFunctionCompiler
    {

        public string FuncName => "size_of";

        #region Public

        public ExpressionTarget Compile( HlCompilation compilation, HlInvocationOp expr )
        {
            if ( expr.ParameterList.Length != 1 )
            {
                EventManager < ErrorEvent >.SendEvent(
                                                      new FunctionArgumentMismatchEvent(
                                                           "Invalid Arguments. Expected size_of(variable)"
                                                          )
                                                     );
            }

            
            if ( compilation.ContainsVariable( expr.ParameterList[0].ToString() ) )
            {
                string v = compilation.GetTempVar(
                                                  compilation.GetVariable( expr.ParameterList[0].ToString() ).Size
                                                 );

                return new ExpressionTarget(
                                            v,
                                            true,
                                            compilation.TypeSystem.GetType(
                                                                           compilation.Root,
                                                                           HLBaseTypeNames.s_UintTypeName
                                                                          )
                                           );
            }

            if ( expr.ParameterList[0] is HlMemberAccessOp mac )
            {
                ExpressionTarget lType = compilation.Parse(mac.Left);

                if ( lType.ResultAddress == "%%TYPE%%" )
                {
                    HlMemberDefinition member = lType.TypeDefinition.
                                                      GetPrivateOrPublicMember(
                                                                               mac.MemberName.ToString()
                                                                              );
                    VariableData var = compilation.
                        GetVariable(
                                    lType.TypeDefinition.GetFinalMemberName(
                                                                           member
                                                                           )
                                   );
                    string v = compilation.GetTempVar(
                                                      var.
                                                          Size
                                                     );

                    return new ExpressionTarget(
                                                v,
                                                true,
                                                compilation.TypeSystem.GetType(
                                                                               compilation.Root,
                                                                               HLBaseTypeNames.s_UintTypeName
                                                                              )
                                               );
                }
                else
                    throw new Exception();
            }

            if ( compilation.TypeSystem.HasType( compilation.Root, expr.ParameterList[0].ToString() ) )
            {
                string v = compilation.GetTempVar(
                                                  compilation.TypeSystem.
                                                              GetType(
                                                                      compilation.Root,
                                                                      expr.ParameterList[0].ToString()
                                                                     ).
                                                              GetSize()
                                                 );

                return new ExpressionTarget(
                                            v,
                                            true,
                                            compilation.TypeSystem.GetType(
                                                                           compilation.Root,
                                                                           HLBaseTypeNames.s_UintTypeName
                                                                          )
                                           );
            }

            EventManager < ErrorEvent >.SendEvent(
                                                  new HlVariableNotFoundEvent(
                                                                              expr.ParameterList[0].ToString(),
                                                                              false
                                                                             )
                                                 );

            return new ExpressionTarget();
        }

        #endregion

    }

}
