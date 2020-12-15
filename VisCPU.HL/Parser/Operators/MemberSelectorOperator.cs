using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Parser.Operators
{
    /// <summary>
    ///     Implements Member Selector Operator
    /// </summary>
    public class MemberSelectorOperator : HLExpressionOperator
    {

        /// <summary>
        ///     Precedence Level of the Operators
        /// </summary>
        public override int PrecedenceLevel => 2;

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public override bool CanCreate(HLExpressionParser parser, HLExpression currentNode)
        {
            return parser.CurrentToken.Type == HLTokenType.OpDot;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override HLExpression Create(HLExpressionParser parser, HLExpression currentNode)
        {
            parser.Eat(HLTokenType.OpDot);
            string name = parser.CurrentToken.ToString();
            parser.Eat(parser.CurrentToken.Type);
            return new HLMemberAccessOp(currentNode, name);
        }

    }
}