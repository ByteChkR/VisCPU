﻿using System.Linq;
using VisCPU.HL.Parser.Events;
using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.Parser.Tokens.Combined;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Parser
{

    /// <summary>
    ///     Implements AXLangExpressionValueCreator
    /// </summary>
    public class HLExpressionValueCreator
    {
        #region Public

        /// <summary>
        ///     Creates a Value based on the Current State of the Expression Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed Expression</returns>
        public HLExpression CreateValue( HLExpressionParser parser )
        {
            if ( parser.CurrentToken.Type == HLTokenType.OpBang || parser.CurrentToken.Type == HLTokenType.OpTilde )
            {
                HLTokenType t = parser.CurrentToken.Type;
                parser.Eat( t );
                HLExpression token = new HLUnaryOp( CreateValue( parser ), t );

                return token;
            }

            if ( parser.CurrentToken.Type == HLTokenType.OpAnd )
            {
                HLTokenType t = parser.CurrentToken.Type;
                parser.Eat( t );
                HLExpression token = new HLUnaryOp( CreateValue( parser ), HLTokenType.OpReference );

                return token;
            }

            if ( parser.CurrentToken.Type == HLTokenType.OpAsterisk )
            {
                HLTokenType t = parser.CurrentToken.Type;
                parser.Eat( t );
                HLExpression token = new HLUnaryOp( CreateValue( parser ), HLTokenType.OpDeReference );

                return token;
            }

            if ( parser.CurrentToken.Type == HLTokenType.OpNew )
            {
                parser.Eat( parser.CurrentToken.Type );
                HLExpression token = new HLUnaryOp( parser.ParseExpr(), HLTokenType.OpNew );

                return token;
            }

            if ( parser.CurrentToken.Type == HLTokenType.OpReturn )
            {
                IHlToken rt = parser.CurrentToken;
                parser.Eat( HLTokenType.OpReturn );

                if ( parser.CurrentToken.Type == HLTokenType.OpSemicolon )
                {
                    return new HLReturnOp( null, rt.SourceIndex );
                }

                return new HLReturnOp( parser.ParseExpr(), rt.SourceIndex );
            }

            if ( parser.CurrentToken.Type == HLTokenType.OpContinue )
            {
                IHlToken ct = parser.CurrentToken;
                parser.Eat( HLTokenType.OpContinue );

                return new HLContinueOp( ct.SourceIndex );
            }

            if ( parser.CurrentToken.Type == HLTokenType.OpBreak )
            {
                IHlToken bt = parser.CurrentToken;
                parser.Eat( HLTokenType.OpBreak );

                return new HLBreakOp( bt.SourceIndex );
            }

            if ( parser.CurrentToken.Type == HLTokenType.OpIf )
            {
                return HLSpecialOps.ReadIf( parser );
            }

            if ( parser.CurrentToken.Type == HLTokenType.OpFor )
            {
                return HLSpecialOps.ReadFor( parser );
            }

            if ( parser.CurrentToken.Type == HLTokenType.OpWhile )
            {
                return HLSpecialOps.ReadWhile( parser );
            }

            if ( parser.CurrentToken.Type == HLTokenType.OpBracketOpen )
            {
                parser.Eat( HLTokenType.OpBracketOpen );
                HLExpression token = parser.ParseExpr();
                parser.Eat( HLTokenType.OpBracketClose );

                return token;
            }

            if ( parser.CurrentToken.Type == HLTokenType.OpThis ||
                 parser.CurrentToken.Type == HLTokenType.OpBase )
            {
                HLExpression token = new HLVarOperand(
                    parser.CurrentToken,
                    parser.CurrentToken.SourceIndex
                );

                parser.Eat( parser.CurrentToken.Type );

                return token;
            }

            if ( parser.CurrentToken.Type == HLTokenType.OpWord )
            {
                IHlToken item = parser.CurrentToken;
                parser.Eat( parser.CurrentToken.Type );

                HLExpression token = new HLVarOperand( item, item.SourceIndex );

                return token;
            }

            if ( parser.CurrentToken.Type == HLTokenType.OpVariableDefinition )
            {
                VariableDefinitionToken vd = ( VariableDefinitionToken ) parser.CurrentToken;
                HLVarDefOperand token;

                if ( vd.InitializerExpression != null && vd.InitializerExpression.Length != 0 )
                {
                    HLExpressionParser p =
                        HLExpressionParser.Create( new HLExpressionReader( vd.InitializerExpression.ToList() ) );

                    token =
                        new HLVarDefOperand( vd, p.Parse() );
                }
                else
                {
                    token =
                        new HLVarDefOperand( vd, new HLExpression[0] );
                }

                parser.Eat( HLTokenType.OpVariableDefinition );

                return token;
            }

            if ( parser.CurrentToken.Type == HLTokenType.OpFunctionDefinition )
            {
                FunctionDefinitionToken fToken = ( FunctionDefinitionToken ) parser.CurrentToken;
                parser.Eat( HLTokenType.OpFunctionDefinition );

                HLExpression token =
                    new HLFuncDefOperand(
                        fToken,
                        HLExpressionParser.Create( new HLExpressionReader( fToken.Block.ToList() ) ).
                                           Parse()
                    );

                return token;
            }

            if ( parser.CurrentToken.Type == HLTokenType.OpNumber ||
                 parser.CurrentToken.Type == HLTokenType.OpStringLiteral ||
                 parser.CurrentToken.Type == HLTokenType.OpCharLiteral )
            {
                HLExpression token = new HLValueOperand( parser.CurrentToken );
                parser.Eat( parser.CurrentToken.Type );

                return token;
            }

            EventManager < ErrorEvent >.SendEvent( new HLTokenReadEvent( HLTokenType.Any, parser.CurrentToken.Type ) );

            return null;
        }

        #endregion
    }

}
