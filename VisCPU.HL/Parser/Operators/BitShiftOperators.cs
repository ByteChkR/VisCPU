using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Parser.Operators
{

    /// <summary>
    ///     Implements Logical AND Operator
    /// </summary>
    public class BitShiftOperators : HLExpressionOperator
    {
        /// <summary>
        ///     Precedence Level of the Operators
        /// </summary>
        public override int PrecedenceLevel => 7;

        #region Public

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public override bool CanCreate( HLExpressionParser parser, HLExpression currentNode )
        {
            return parser.CurrentToken.Type == HLTokenType.OpLessThan &&
                   parser.Reader.PeekNext().Type == HLTokenType.OpLessThan ||
                   parser.CurrentToken.Type == HLTokenType.OpGreaterThan &&
                   parser.Reader.PeekNext().Type == HLTokenType.OpGreaterThan;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override HLExpression Create( HLExpressionParser parser, HLExpression currentNode )
        {
            HLTokenType type = parser.CurrentToken.Type;
            parser.Eat( type );
            parser.Eat( type );

            if ( type == HLTokenType.OpLessThan )
            {
                type = HLTokenType.OpShiftLeft;
            }
            else if ( type == HLTokenType.OpGreaterThan )
            {
                type = HLTokenType.OpShiftRight;
            }

            return new HLBinaryOp(
                currentNode,
                type,
                parser.ParseExpr( PrecedenceLevel )
            );
        }

        #endregion
    }

}
