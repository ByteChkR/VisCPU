using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Parser.Operators
{

    /// <summary>
    ///     Implements Unary Plus/Minus by Assignment Operators
    /// </summary>
    public class AssignmentPlusMinusOperators : HlExpressionOperator
    {
        /// <summary>
        ///     Precedence Level of the Operators
        /// </summary>
        public override int PrecedenceLevel => 2;

        #region Public

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public override bool CanCreate( HlExpressionParser parser, HlExpression currentNode )
        {
            return parser.CurrentToken.Type == HlTokenType.OpPlus &&
                   parser.Reader.PeekNext().Type == HlTokenType.OpPlus ||
                   parser.CurrentToken.Type == HlTokenType.OpMinus &&
                   parser.Reader.PeekNext().Type == HlTokenType.OpMinus;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override HlExpression Create( HlExpressionParser parser, HlExpression currentNode )
        {
            HlTokenType tt = parser.CurrentToken.Type == HlTokenType.OpPlus
                ? HlTokenType.OpUnaryIncrement
                : HlTokenType.OpUnaryDecrement;

            parser.Eat( parser.CurrentToken.Type );
            parser.Eat( parser.CurrentToken.Type );

            HlExpression token =
                new HlUnaryOp( currentNode, tt );

            return token;
        }

        #endregion
    }

}
