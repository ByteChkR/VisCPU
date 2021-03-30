using System;
using System.Collections.Generic;
using System.Linq;
using VisCPU.HL.DataTypes;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.Parser.Tokens.Combined;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;

namespace VisCPU.HL.Compiler.Types
{

    public class FunctionDefinitionCompiler : HlExpressionCompiler < HlFuncDefOperand >
    {
        #region Public

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlFuncDefOperand expr )
        {
            bool isPublic = expr.FunctionDefinition.Mods.Any( x => x.Type == HlTokenType.OpPublicMod );
            bool isStatic = expr.FunctionDefinition.Mods.Any( x => x.Type == HlTokenType.OpStaticMod );
            bool isAbstract = expr.FunctionDefinition.Mods.Any( x => x.Type == HlTokenType.OpAbstractMod );

            string funcName = expr.FunctionDefinition.Parent == null
                ? expr.FunctionDefinition.FunctionName.ToString()
                : $"{expr.FunctionDefinition.Parent.Name}_{expr.FunctionDefinition.FunctionName}";

            HlCompilation fComp = new HlCompilation( compilation, funcName, compilation.Root );
            Func < string[] > compiler = null;

            if ( !isAbstract )
            {
                compiler = () =>
                {
                    foreach ( IHlToken valueArgument in expr.
                                                        FunctionDefinition.Arguments )
                    {
                        VariableDefinitionToken vdef =
                            valueArgument as VariableDefinitionToken;

                        string key = vdef.Name.ToString();

                        fComp.CreateVariable(
                            key,
                            1,
                            compilation.TypeSystem.GetType(
                                compilation.Root,
                                vdef.TypeName.ToString()
                            ),
                            false,
                            false
                        );
                    }

                    List < string > parsedVal =
                        fComp.Parse( expr.Block, false, null ).
                              Replace( "\r", "" ).
                              Split( '\n' ).
                              ToList();

                    foreach ( IHlToken valueArgument in expr.
                                                        FunctionDefinition.Arguments )
                    {
                        parsedVal.Insert(
                            0,
                            $"POP {fComp.GetFinalName( ( valueArgument as VariableDefinitionToken ).Name.ToString() )}"
                        );
                    }

                    parsedVal.Add(
                        "PUSH 0 ; Push anything. Will not be used anyway."
                    );

                    parsedVal.Add( "RET ; Compiler Safeguard." );
                    parsedVal.Insert( 0, "." + funcName + ( isPublic ? "" : " linker:hide" ) );

                    return parsedVal.ToArray();
                };
            }

            compilation.FunctionMap.Set(
                funcName,
                new FunctionData(
                    funcName,
                    isPublic,
                    isStatic,
                    compiler,
                    expr.FunctionDefinition.Arguments.Length,
                    expr.FunctionDefinition.FunctionReturnType.ToString()
                )
            );

            return new ExpressionTarget();
        }

        #endregion
    }

}
