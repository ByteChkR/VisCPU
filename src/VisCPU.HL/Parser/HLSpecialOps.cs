using System.Collections.Generic;
using System.Linq;
using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Parser
{

    /// <summary>
    ///     Implements Special Expressions that do require some custom parsing steps
    /// </summary>
    public static class HlSpecialOps
    {
        #region Public

        /// <summary>
        ///     Parses a For Expression from the Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed Expression</returns>
        public static HlExpression ReadFor( HlExpressionParser parser )
        {
            IHlToken ft = parser.CurrentToken;
            parser.Eat( HlTokenType.OpFor );
            parser.Eat( HlTokenType.OpBracketOpen );
            HlExpression vDecl = parser.ParseExpr();
            HlExpression condition = parser.ParseExpr();
            HlExpression vInc = parser.ParseExpr();
            parser.Eat( HlTokenType.OpBracketClose );

            HlExpression token = null;
            List < HlExpression > block;

            if ( parser.CurrentToken.Type != HlTokenType.OpBlockToken )
            {
                block = new List < HlExpression > { parser.ParseExpr() };
            }
            else
            {
                block = HlExpressionParser.Create( new HlExpressionReader( parser.CurrentToken.GetChildren() ) ).
                                           Parse().
                                           ToList();
            }

            token = new HlForOp( vDecl, condition, vInc, block.ToArray(), ft.SourceIndex );

            return token;
        }

        /// <summary>
        ///     Parses an If Expression from the Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed If Expression</returns>
        public static HlExpression ReadIf( HlExpressionParser parser )
        {
            List < (HlExpression, HlExpression[]) > conditions =
                new List < (HlExpression, HlExpression[]) >();

            HlExpression[] elseBranch = null;
            IHlToken it = parser.CurrentToken;
            conditions.Add( ReadIfStatement( parser ) );

            while ( parser.CurrentToken.Type == HlTokenType.OpElse )
            {
                parser.Eat( HlTokenType.OpElse );

                if ( parser.CurrentToken.Type == HlTokenType.OpIf )
                {
                    conditions.Add( ReadIfStatement( parser ) );
                }
                else
                {
                    List < HlExpression > content = ReadIfBlockContent( parser );
                    elseBranch = content.ToArray();

                    break;
                }
            }

            return new HlIfOp( conditions, elseBranch, it.SourceIndex );
        }

        /// <summary>
        ///     Parses a While Expression from the Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed Expression</returns>
        public static HlExpression ReadWhile( HlExpressionParser parser )
        {
            IHlToken wT = parser.CurrentToken;
            parser.Eat( HlTokenType.OpWhile );
            parser.Eat( HlTokenType.OpBracketOpen );
            HlExpression condition = parser.ParseExpr();
            parser.Eat( HlTokenType.OpBracketClose );

            HlExpression token = null;
            List < HlExpression > block;

            if ( parser.CurrentToken.Type != HlTokenType.OpBlockToken )
            {
                block = new List < HlExpression > { parser.ParseExpr() };
            }
            else
            {
                block = HlExpressionParser.Create( new HlExpressionReader( parser.CurrentToken.GetChildren() ) ).
                                           Parse().
                                           ToList();
            }

            token = new HlWhileOp( condition, block.ToArray(), wT.SourceIndex );

            return token;
        }

        #endregion

        #region Private

        /// <summary>
        ///     Parses an If Expression block from the Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed Expression Block</returns>
        private static List < HlExpression > ReadIfBlockContent( HlExpressionParser parser )
        {
            if ( parser.CurrentToken.Type != HlTokenType.OpBlockToken )
            {
                HlExpression expr = parser.ParseExpr();
                parser.Eat( HlTokenType.OpSemicolon );

                return new List < HlExpression > { expr };
            }

            IHlToken token = parser.CurrentToken;
            parser.Eat( HlTokenType.OpBlockToken );

            return HlExpressionParser.Create( new HlExpressionReader( token.GetChildren() ) ).Parse().ToList();
        }

        /// <summary>
        ///     Parses an If Expression from the Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed Condition Expression and Expression Block</returns>
        private static (HlExpression, HlExpression[]) ReadIfStatement( HlExpressionParser parser )
        {
            parser.Eat( HlTokenType.OpIf );
            parser.Eat( HlTokenType.OpBracketOpen );
            HlExpression condition = parser.ParseExpr();
            parser.Eat( HlTokenType.OpBracketClose );

            List < HlExpression > content = ReadIfBlockContent( parser );

            return ( condition, content.ToArray() );
        }

        #endregion
    }

}
