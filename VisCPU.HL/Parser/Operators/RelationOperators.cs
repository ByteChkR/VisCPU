using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Parser.Operators
{
    /// <summary>
    ///     Implements LessThan/GreaterThan Operators
    /// </summary>
    public class RelationOperators : HLExpressionOperator
    {

        /// <summary>
        ///     Precedence Level of the Operators
        /// </summary>
        public override int PrecedenceLevel => 8;

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public override bool CanCreate(HLExpressionParser parser, HLExpression currentNode)
        {
            return parser.CurrentToken.Type == HLTokenType.OpLessThan ||
                   parser.CurrentToken.Type == HLTokenType.OpGreaterThan;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override HLExpression Create(HLExpressionParser parser, HLExpression currentNode)
        {
            HLTokenType type = parser.CurrentToken.Type;
            parser.Eat(parser.CurrentToken.Type);
            HLExpression node = null;

            if (type == HLTokenType.OpLessThan)
            {
                if (parser.CurrentToken.Type == HLTokenType.OpEquality)
                {
                    parser.Eat(HLTokenType.OpEquality);
                    node = new HLBinaryOp(
                                          currentNode,
                                          HLTokenType.OpLessOrEqual,
                                          parser.ParseExpr(0)
                                         );
                }
                else
                {
                    node = new HLBinaryOp(currentNode, type, parser.ParseExpr(0));
                }
            }
            else if (type == HLTokenType.OpGreaterThan)
            {
                if (parser.CurrentToken.Type == HLTokenType.OpEquality)
                {
                    parser.Eat(HLTokenType.OpEquality);
                    node = new HLBinaryOp(
                                          currentNode,
                                          HLTokenType.OpGreaterOrEqual,
                                          parser.ParseExpr(0)
                                         );
                }
                else
                {
                    node = new HLBinaryOp(currentNode, type, parser.ParseExpr(0));
                }
            }

            return node;
        }

    }
}