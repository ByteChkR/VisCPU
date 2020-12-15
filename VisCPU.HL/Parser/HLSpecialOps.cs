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
    public static class HLSpecialOps
    {

        #region For Parser

        /// <summary>
        ///     Parses a For Expression from the Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed Expression</returns>
        public static HLExpression ReadFor(HLExpressionParser parser)
        {
            IHLToken ft = parser.CurrentToken;
            parser.Eat(HLTokenType.OpFor);
            parser.Eat(HLTokenType.OpBracketOpen);
            HLExpression vDecl = parser.ParseExpr(0);
            parser.Eat(HLTokenType.OpSemicolon);
            HLExpression condition = parser.ParseExpr(0);
            parser.Eat(HLTokenType.OpSemicolon);
            HLExpression vInc = parser.ParseExpr(0);
            parser.Eat(HLTokenType.OpBracketClose);

            HLExpression token = null;
            List<HLExpression> block;
            if (parser.CurrentToken.Type != HLTokenType.OpBlockToken)
            {
                block = new List<HLExpression> { parser.ParseExpr(0) };
            }
            else
            {
                block = HLExpressionParser
                        .Create(new HLExpressionReader(parser.CurrentToken.GetChildren())).Parse()
                        .ToList();
            }

            token = new HLForOp(vDecl, condition, vInc, block.ToArray(), ft.SourceIndex);


            return token;
        }

        #endregion


        #region While Parser

        /// <summary>
        ///     Parses a While Expression from the Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed Expression</returns>
        public static HLExpression ReadWhile(HLExpressionParser parser)
        {
            IHLToken wT = parser.CurrentToken;
            parser.Eat(HLTokenType.OpWhile);
            parser.Eat(HLTokenType.OpBracketOpen);
            HLExpression condition = parser.ParseExpr(0);
            parser.Eat(HLTokenType.OpBracketClose);

            HLExpression token = null;
            List<HLExpression> block;
            if (parser.CurrentToken.Type != HLTokenType.OpBlockToken)
            {
                block = new List<HLExpression> { parser.ParseExpr(0) };
            }
            else
            {
                block = HLExpressionParser
                        .Create(new HLExpressionReader(parser.CurrentToken.GetChildren())).Parse()
                        .ToList();
            }

            token = new HLWhileOp(condition, block.ToArray(), wT.SourceIndex);

            return token;
        }

        #endregion


        #region If Parser

        /// <summary>
        ///     Parses an If Expression from the Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed Condition Expression and Expression Block</returns>
        private static (HLExpression, HLExpression[]) ReadIfStatement(
            HLExpressionParser parser)
        {
            parser.Eat(HLTokenType.OpIf);
            parser.Eat(HLTokenType.OpBracketOpen);
            HLExpression condition = parser.ParseExpr(0);
            parser.Eat(HLTokenType.OpBracketClose);

            List<HLExpression> content = ReadIfBlockContent(parser);
            return (condition, content.ToArray());
        }

        /// <summary>
        ///     Parses an If Expression block from the Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed Expression Block</returns>
        private static List<HLExpression> ReadIfBlockContent(HLExpressionParser parser)
        {
            if (parser.CurrentToken.Type != HLTokenType.OpBlockToken)
            {
                HLExpression expr = parser.ParseExpr(0);
                parser.Eat(HLTokenType.OpSemicolon);
                return new List<HLExpression> { expr };
            }

            IHLToken token = parser.CurrentToken;
            parser.Eat(HLTokenType.OpBlockToken);

            return HLExpressionParser.Create(new HLExpressionReader(token.GetChildren()))
                                     .Parse().ToList();
        }


        /// <summary>
        ///     Parses an If Expression from the Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed If Expression</returns>
        public static HLExpression ReadIf(HLExpressionParser parser)
        {
            List<(HLExpression, HLExpression[])> conditions =
                new List<(HLExpression, HLExpression[])>();
            HLExpression[] elseBranch = null;
            IHLToken it = parser.CurrentToken;
            conditions.Add(ReadIfStatement(parser));

            while (parser.CurrentToken.Type == HLTokenType.OpElse)
            {
                parser.Eat(HLTokenType.OpElse);
                if (parser.CurrentToken.Type == HLTokenType.OpIf)
                {
                    conditions.Add(ReadIfStatement(parser));
                }
                else
                {
                    List<HLExpression> content = ReadIfBlockContent(parser);
                    elseBranch = content.ToArray();
                    break;
                }
            }


            return new HLIfOp(conditions, elseBranch, it.SourceIndex);
        }

        #endregion

    }
}