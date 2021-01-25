﻿using System.Linq;
using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Events;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

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
                    expr.VariableDefinition.Modifiers.Any(
                        x => x.Type == HlTokenType.OpPublicMod
                    )
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

                if ( compilation.ConstValTypes.ContainsKey( asmVarName ) )
                {
                    EventManager < ErrorEvent >.SendEvent( new DuplicateConstVarDefinitionEvent( asmVarName ) );
                }

                compilation.ConstValTypes.Add(
                    asmVarName,
                    expr.VariableDefinition.InitializerExpression.FirstOrDefault()?.ToString()
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
