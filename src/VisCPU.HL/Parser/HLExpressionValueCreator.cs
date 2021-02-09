using System.Linq;

using VisCPU.HL.Parser.Events;
using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.Parser.Tokens.Combined;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Parser
{

    /// <summary>
    ///     Implements AXLangExpressionValueCreator
    /// </summary>
    public class HlExpressionValueCreator
    {

        #region Public

        /// <summary>
        ///     Creates a Value based on the Current State of the Expression Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed Expression</returns>
        public HlExpression CreateValue( HlExpressionParser parser, uint maxPrecedence )
        {
            if ( parser.CurrentToken.Type == HlTokenType.OpBang ||
                 parser.CurrentToken.Type == HlTokenType.OpTilde ||
                 parser.CurrentToken.Type == HlTokenType.OpPlus ||
                 parser.CurrentToken.Type == HlTokenType.OpMinus )
            {
                HlTokenType t = parser.CurrentToken.Type;
                parser.Eat( t );
                HlExpression token = new HlUnaryOp( parser.ParseExpr(), t );

                return token;
            }

            if ( parser.CurrentToken.Type == HlTokenType.OpAnd )
            {
                HlTokenType t = parser.CurrentToken.Type;
                parser.Eat( t );
                HlExpression token = new HlUnaryOp( parser.ParseExpr( 2 ), HlTokenType.OpReference );

                return token;
            }

            if ( parser.CurrentToken.Type == HlTokenType.OpAsterisk )
            {
                HlTokenType t = parser.CurrentToken.Type;
                parser.Eat( t );
                HlExpression token = new HlUnaryOp( parser.ParseExpr( 2 ), HlTokenType.OpDeReference );

                return token;
            }

            if ( parser.CurrentToken.Type == HlTokenType.OpNew && maxPrecedence >= 3)
            {
                parser.Eat( parser.CurrentToken.Type );
                HlExpression token = new HlUnaryOp( parser.ParseExpr(2), HlTokenType.OpNew );

                return token;
            }

            if ( parser.CurrentToken.Type == HlTokenType.OpReturn )
            {
                IHlToken rt = parser.CurrentToken;
                parser.Eat( HlTokenType.OpReturn );

                if ( parser.CurrentToken.Type == HlTokenType.OpSemicolon )
                {
                    return new HlReturnOp( null, rt.SourceIndex );
                }

                return new HlReturnOp( parser.ParseExpr(), rt.SourceIndex );
            }

            if ( parser.CurrentToken.Type == HlTokenType.OpContinue )
            {
                IHlToken ct = parser.CurrentToken;
                parser.Eat( HlTokenType.OpContinue );

                return new HlContinueOp( ct.SourceIndex );
            }

            if ( parser.CurrentToken.Type == HlTokenType.OpBreak )
            {
                IHlToken bt = parser.CurrentToken;
                parser.Eat( HlTokenType.OpBreak );

                return new HlBreakOp( bt.SourceIndex );
            }

            if ( parser.CurrentToken.Type == HlTokenType.OpIf )
            {
                return HlSpecialOps.ReadIf( parser );
            }

            if ( parser.CurrentToken.Type == HlTokenType.OpFor )
            {
                return HlSpecialOps.ReadFor( parser );
            }

            if ( parser.CurrentToken.Type == HlTokenType.OpWhile )
            {
                return HlSpecialOps.ReadWhile( parser );
            }

            if ( parser.CurrentToken.Type == HlTokenType.OpBracketOpen )
            {
                parser.Eat( HlTokenType.OpBracketOpen );
                HlExpression token = parser.ParseExpr();
                parser.Eat( HlTokenType.OpBracketClose );

                return token;
            }

            if ( parser.CurrentToken.Type == HlTokenType.OpThis ||
                 parser.CurrentToken.Type == HlTokenType.OpBase )
            {
                HlExpression token = new HlVarOperand(
                                                      parser.CurrentToken,
                                                      parser.CurrentToken.SourceIndex
                                                     );

                parser.Eat( parser.CurrentToken.Type );

                return token;
            }

            if ( parser.CurrentToken.Type == HlTokenType.OpWord || parser.CurrentToken.Type == HlTokenType.OpNew)
            {
                IHlToken item = parser.CurrentToken;
                parser.Eat( parser.CurrentToken.Type );

                HlExpression token = new HlVarOperand( item, item.SourceIndex );

                return token;
            }

            if ( parser.CurrentToken.Type == HlTokenType.OpVariableDefinition )
            {
                VariableDefinitionToken vd = ( VariableDefinitionToken ) parser.CurrentToken;
                HlVarDefOperand token;

                if ( vd.InitializerExpression != null && vd.InitializerExpression.Length != 0 )
                {
                    HlExpressionParser p =
                        HlExpressionParser.Create( new HlExpressionReader( vd.InitializerExpression.ToList() ) );

                    token =
                        new HlVarDefOperand( vd, p.Parse() );
                }
                else
                {
                    token =
                        new HlVarDefOperand( vd, new HlExpression[0] );
                }

                parser.Eat( HlTokenType.OpVariableDefinition );

                return token;
            }

            if ( parser.CurrentToken.Type == HlTokenType.OpFunctionDefinition )
            {
                FunctionDefinitionToken fToken = ( FunctionDefinitionToken ) parser.CurrentToken;
                parser.Eat( HlTokenType.OpFunctionDefinition );

                HlExpression[] expressionBlock = null;

                if ( fToken.Mods.All( x => x.Type != HlTokenType.OpAbstractMod ) )
                {
                    expressionBlock = HlExpressionParser.Create( new HlExpressionReader( fToken.Block.ToList() ) ).
                                                         Parse();
                }

                HlExpression token =
                    new HlFuncDefOperand(
                                         fToken,
                                         expressionBlock
                                        );

                return token;
            }

            if ( parser.CurrentToken.Type == HlTokenType.OpNumber ||
                 parser.CurrentToken.Type == HlTokenType.OpDecimalNumber ||
                 parser.CurrentToken.Type == HlTokenType.OpStringLiteral ||
                 parser.CurrentToken.Type == HlTokenType.OpCharLiteral )
            {
                HlExpression token = new HlValueOperand( parser.CurrentToken );
                parser.Eat( parser.CurrentToken.Type );

                return token;
            }

            EventManager < ErrorEvent >.SendEvent( new HlTokenReadEvent( HlTokenType.Any, parser.CurrentToken.Type ) );

            return null;
        }

        #endregion

    }

}
