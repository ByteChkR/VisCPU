﻿using System.Collections.Generic;
using System.Linq;

using VisCPU.HL.DataTypes;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.Parser.Tokens.Combined;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;

namespace VisCPU.HL.Compiler.Types
{

    public class FunctionDefinitionCompiler : HLExpressionCompiler < HLFuncDefOperand >
    {

        #region Public

        public override ExpressionTarget ParseExpression( HLCompilation compilation, HLFuncDefOperand expr )
        {
            bool isPublic = expr.value.Mods.Any( x => x.ToString() == "public" );

            HLCompilation fComp = new HLCompilation( compilation, expr.value.FunctionName.ToString() );

            compilation.FunctionMap[expr.value.FunctionName.ToString()] = new FunctionData(
                 expr.value.FunctionName.ToString(),
                 isPublic,
                 () =>
                 {
                     //Log( $"Importing Function: {expr.value.FunctionName}" );

                     foreach ( IHLToken valueArgument in expr.value.Arguments )
                     {
                         VariableDefinitionToken vdef = valueArgument as VariableDefinitionToken;
                         string key = vdef.Name.ToString();
                         fComp.CreateVariable( key, 1, compilation.TypeSystem.GetType( vdef.TypeName.ToString() ), false );
                     }

                     List < string > parsedVal =
                         fComp.Parse( expr.Block, false, null ).Replace( "\r", "" ).Split( '\n' ).ToList();

                     foreach ( IHLToken valueArgument in expr.value.Arguments )
                     {
                         parsedVal.Insert(
                                          0,
                                          $"POP {fComp.GetFinalName( ( valueArgument as VariableDefinitionToken ).Name.ToString() )}"
                                         );
                     }

                     parsedVal.Add( "RET ; Compiler Safeguard." );

                     return parsedVal.ToArray();
                 },
                 expr.value.Arguments.Length,
                 expr.value.FunctionReturnType.Type != HLTokenType.OpTypeVoid
                );

            return new ExpressionTarget();
        }

        #endregion

    }

}