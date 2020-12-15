using System;

using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Parser.Operators
{
    /// <summary>
    ///     Implements Assignment by Sum and Difference
    /// </summary>
    public class AssignmentByOperators : HLExpressionOperator
    {

        /// <summary>
        ///     Precedence Level of the Operators
        /// </summary>
        public override int PrecedenceLevel => 15;

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public override bool CanCreate(HLExpressionParser parser, HLExpression currentNode)
        {
            return (parser.CurrentToken.Type == HLTokenType.OpPlus ||
                    parser.CurrentToken.Type == HLTokenType.OpMinus ||
                    parser.CurrentToken.Type == HLTokenType.OpAsterisk ||
                    parser.CurrentToken.Type == HLTokenType.OpFwdSlash ||
                    parser.CurrentToken.Type == HLTokenType.OpPercent ||
                    parser.CurrentToken.Type == HLTokenType.OpAnd ||
                    parser.CurrentToken.Type == HLTokenType.OpPipe ||
                    parser.CurrentToken.Type == HLTokenType.OpCap) &&
                   parser.Reader.PeekNext().Type == HLTokenType.OpEquality;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override HLExpression Create(HLExpressionParser parser, HLExpression currentNode)
        {
            IHLToken token = parser.CurrentToken;
            HLTokenType tt;

            switch (parser.CurrentToken.Type)
            {
                case HLTokenType.OpPlus:
                    tt = HLTokenType.OpSumAssign;
                    break;
                case HLTokenType.OpMinus:
                    tt = HLTokenType.OpDifAssign;
                    break;
                case HLTokenType.OpAsterisk:
                    tt = HLTokenType.OpProdAssign;
                    break;
                case HLTokenType.OpFwdSlash:
                    tt = HLTokenType.OpQuotAssign;
                    break;
                case HLTokenType.OpPercent:
                    tt = HLTokenType.OpRemAssign;
                    break;
                case HLTokenType.OpPipe:
                    tt = HLTokenType.OpOrAssign;
                    break;
                case HLTokenType.OpAnd:
                    tt = HLTokenType.OpAndAssign;
                    break;
                case HLTokenType.OpCap:
                    tt = HLTokenType.OpXOrAssign;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            parser.Eat(token.Type);
            parser.Eat(HLTokenType.OpEquality);
            return new HLBinaryOp(currentNode, tt, parser.ParseExpr(0));
        }

    }
}