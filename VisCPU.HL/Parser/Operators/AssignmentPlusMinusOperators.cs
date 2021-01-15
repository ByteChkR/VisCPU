using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Parser.Operators
{

    /// <summary>
    ///     Implements Unary Plus/Minus by Assignment Operators
    /// </summary>
    public class AssignmentPlusMinusOperators : HLExpressionOperator
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
        public override bool CanCreate( HLExpressionParser parser, HLExpression currentNode )
        {
            return parser.CurrentToken.Type == HLTokenType.OpPlus &&
                   parser.Reader.PeekNext().Type == HLTokenType.OpPlus ||
                   parser.CurrentToken.Type == HLTokenType.OpMinus &&
                   parser.Reader.PeekNext().Type == HLTokenType.OpMinus;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override HLExpression Create( HLExpressionParser parser, HLExpression currentNode )
        {
            HLTokenType tt = parser.CurrentToken.Type == HLTokenType.OpPlus
                                 ? HLTokenType.OpUnaryIncrement
                                 : HLTokenType.OpUnaryDecrement;

            parser.Eat( parser.CurrentToken.Type );
            parser.Eat( parser.CurrentToken.Type );

            HLExpression token =
                new HLUnaryOp( currentNode, tt );

            return token;
        }

        #endregion

    }

}
