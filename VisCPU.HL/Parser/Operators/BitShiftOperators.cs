using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Parser.Operators
{

    /// <summary>
    ///     Implements Logical AND Operator
    /// </summary>
    public class BitShiftOperators : HlExpressionOperator
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
        public override bool CanCreate( HlExpressionParser parser, HlExpression currentNode )
        {
            return parser.CurrentToken.Type == HlTokenType.OpLessThan &&
                   parser.Reader.PeekNext().Type == HlTokenType.OpLessThan ||
                   parser.CurrentToken.Type == HlTokenType.OpGreaterThan &&
                   parser.Reader.PeekNext().Type == HlTokenType.OpGreaterThan;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override HlExpression Create( HlExpressionParser parser, HlExpression currentNode )
        {
            HlTokenType type = parser.CurrentToken.Type;
            parser.Eat( type );
            parser.Eat( type );

            if ( type == HlTokenType.OpLessThan )
            {
                type = HlTokenType.OpShiftLeft;
            }
            else if ( type == HlTokenType.OpGreaterThan )
            {
                type = HlTokenType.OpShiftRight;
            }

            return new HlBinaryOp(
                currentNode,
                type,
                parser.ParseExpr( PrecedenceLevel )
            );
        }

        #endregion
    }

}
