using System.Linq;

using VisCPU.Events;
using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Compiler.Variables
{

    public class VariableDefinitionExpressionCompiler : HLExpressionCompiler < HLVarDefOperand >
    {

        public readonly HLTypeSystem TypeSystem;

        #region Public

        public VariableDefinitionExpressionCompiler( HLTypeSystem typeSystem )
        {
            TypeSystem = typeSystem;
        }

        public override ExpressionTarget ParseExpression( HLCompilation compilation, HLVarDefOperand expr )
        {
            if ( expr.value.Modifiers.All( x => x.Type != HLTokenType.OpConstMod ) )
            {
                string asmVarName = expr.value.Name.ToString();

                if ( compilation.ContainsLocalVariable( asmVarName ) )
                {
                    EventManager < ErrorEvent >.SendEvent( new DuplicateVarDefinitionEvent( asmVarName ) );
                }

                HLTypeDefinition vdef = TypeSystem.GetType( expr.value.TypeName.ToString() );
                uint arrSize = expr.value.Size?.ToString().ParseUInt() ?? 1;

                if ( arrSize != 1 )
                {
                    vdef = new ArrayTypeDefintion( vdef, arrSize );
                }

                HLExpression init = expr.Initializer.FirstOrDefault();

                if ( init != null )
                {
                    if ( init is HLValueOperand vOp &&
                         vOp.Value.Type == HLTokenType.OpStringLiteral )
                    {
                        ExpressionTarget svar = new ExpressionTarget(
                                                                     compilation.GetFinalName( asmVarName ),
                                                                     true,
                                                                     vdef
                                                                    );

                        string content = vOp.Value.ToString();
                        compilation.CreateVariable( asmVarName, content, vdef, false );

                        return new ExpressionTarget(
                                                    compilation.GetFinalName( svar.ResultAddress ),
                                                    true,
                                                    vdef
                                                   );
                    }
                }

                ExpressionTarget dvar = new ExpressionTarget(
                                                             compilation.GetFinalName( asmVarName ),
                                                             true,
                                                             vdef
                                                            );

                compilation.CreateVariable(
                                           asmVarName,
                                           arrSize,
                                           vdef,
                                           expr.value.Modifiers.Any( x => x.Type == HLTokenType.OpPublicMod )
                                          );

                if ( init != null )
                {
                    return compilation.Parse( init, dvar ).CopyIfNotNull( compilation, dvar, true );
                }

                return dvar;
            }

            if ( expr.value.Modifiers.Any( x => x.Type == HLTokenType.OpConstMod ) )
            {
                string asmVarName = expr.value.Name.ToString();

                if ( compilation.ConstValTypes.ContainsKey( asmVarName ) )
                {
                    EventManager < ErrorEvent >.SendEvent( new DuplicateConstVarDefinitionEvent( asmVarName ) );
                }

                compilation.ConstValTypes.Add(
                                              asmVarName,
                                              expr.value.InitializerExpression.FirstOrDefault()?.ToString()
                                             );

                return new ExpressionTarget(
                                            asmVarName,
                                            true,
                                            compilation.TypeSystem.GetType( expr.value.TypeName.ToString() )
                                           );
            }

            EventManager < ErrorEvent >.SendEvent( new DynamicVariablesNotSupportedEvent() );

            return new ExpressionTarget();
        }

        #endregion

    }

}
