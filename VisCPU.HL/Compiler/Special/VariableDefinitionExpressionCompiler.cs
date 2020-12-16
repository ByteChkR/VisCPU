﻿using System;

using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.Utility;
using VisCPU.Utility.Events;

namespace VisCPU.HL.Compiler
{

    public class VariableDefinitionExpressionCompiler : HLExpressionCompiler<HLVarDefOperand>
    {

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation, HLVarDefOperand expr)
        {
            if (expr.value.TypeName.ToString() == HLCompilation.CONST_VAL_TYPE)
            {
                string asmVarName = expr.value.Name.ToString();

                if ( compilation.ConstValTypes.ContainsKey( asmVarName ) )
                    EventManager < ErrorEvent >.SendEvent( new DuplicateConstVarDefinitionEvent( asmVarName ) );
                compilation.ConstValTypes.Add(asmVarName, null);

                return new ExpressionTarget(asmVarName, true);
            }

            if (expr.value.TypeName.ToString() == HLCompilation.VAL_TYPE)
            {
                string asmVarName = expr.value.Name.ToString();
                if (compilation.ContainsLocalVariable(asmVarName))
                    EventManager<ErrorEvent>.SendEvent(new DuplicateVarDefinitionEvent(asmVarName));
                compilation.CreateVariable(asmVarName, expr.value.Size?.ToString().ParseUInt() ?? 1);

                return new ExpressionTarget(compilation.GetFinalName(asmVarName), true);
            }

            EventManager < ErrorEvent >.SendEvent( new TypeNotFoundEvent( expr.value.TypeName.ToString() ) );

            return new ExpressionTarget();
        }

    }
}