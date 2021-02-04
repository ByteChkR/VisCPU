using System.Linq;

using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Events;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Compiler.Variables
{

    public class VariableDefinitionExpressionCompiler : HlExpressionCompiler < HlVarDefOperand >
    {

        private readonly HlTypeSystem m_TypeSystem;

        #region Public

        public VariableDefinitionExpressionCompiler( HlTypeSystem typeSystem )
        {
            m_TypeSystem = typeSystem;
        }

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlVarDefOperand expr )
        {
            if ( expr.VariableDefinition.Modifiers.All( x => x.Type != HlTokenType.OpConstMod ) )
            {
                string asmVarName = expr.VariableDefinition.Name.ToString();

                if ( compilation.ContainsLocalVariable( asmVarName ) )
                {
                    EventManager < ErrorEvent >.SendEvent( new DuplicateVarDefinitionEvent( asmVarName ) );
                }

                HlTypeDefinition vdef = m_TypeSystem.GetType( expr.VariableDefinition.TypeName.ToString() );
                uint arrSize = expr.VariableDefinition.Size?.ToString().ParseUInt() ?? 1;

                if ( arrSize != 1 )
                {
                    vdef = new ArrayTypeDefintion( vdef, arrSize );
                }

                compilation.CreateVariable(
                                           asmVarName,
                                           arrSize,
                                           vdef,
                                           expr.VariableDefinition.Modifiers.Any(
                                                x => x.Type == HlTokenType.OpPublicMod
                                               ), false
                                          );

                HlExpression init = expr.Initializer.FirstOrDefault();

                if ( init != null )
                {
                    if ( init is HlValueOperand vOp &&
                         vOp.Value.Type == HlTokenType.OpStringLiteral )
                    {
                        ExpressionTarget svar = new ExpressionTarget(
                                                                     compilation.GetFinalName( asmVarName ),
                                                                     true,
                                                                     vdef
                                                                    );

                        string content = vOp.Value.ToString();
                        compilation.CreateVariable( asmVarName, content, vdef, false, false);

                        return new ExpressionTarget(
                                                     svar.ResultAddress ,
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

               

                if ( init != null )
                {
                    return compilation.Parse( init, dvar ).CopyIfNotNull( compilation, dvar );
                }

                return dvar;
            }

            if ( expr.VariableDefinition.Modifiers.Any( x => x.Type == HlTokenType.OpConstMod ) )
            {
                string asmVarName = expr.VariableDefinition.Name.ToString();

                if ( compilation.ConstValTypes.Contains( asmVarName ) )
                {
                    EventManager < ErrorEvent >.SendEvent( new DuplicateConstVarDefinitionEvent( asmVarName ) );
                }

                compilation.ConstValTypes.Set(
                                              asmVarName,
                                              new ConstantValueItem()
                                              {
                                                  Value = expr.VariableDefinition.InitializerExpression.
                                                               FirstOrDefault()?.
                                                               ToString(),
                                                  IsPublic = expr.VariableDefinition.Modifiers.Any(
                                                       x => x.Type == HlTokenType.OpPublicMod
                                                      )
                                              }
                                             );

                return new ExpressionTarget(
                                            asmVarName,
                                            true,
                                            compilation.TypeSystem.GetType(
                                                                           expr.VariableDefinition.TypeName.ToString()
                                                                          )
                                           );
            }

            EventManager < ErrorEvent >.SendEvent( new DynamicVariablesNotSupportedEvent() );

            return new ExpressionTarget();
        }

        #endregion

    }

}
