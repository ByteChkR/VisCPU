using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Parser.Operators
{

    /// <summary>
    ///     Implements Multiply/Divide Operators
    /// </summary>
    public class MulDivModOperators : HlExpressionOperator
    {

        /// <summary>
        ///     Precedence Level of the Operators
        /// </summary>
        public override int PrecedenceLevel => 5;

        #region Public

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public override bool CanCreate( HlExpressionParser parser, HlExpression currentNode )
        {
            return ( parser.CurrentToken.Type == HlTokenType.OpAsterisk ||
                     parser.CurrentToken.Type == HlTokenType.OpFwdSlash ||
                     parser.CurrentToken.Type == HlTokenType.OpPercent ) &&
                   parser.Reader.PeekNext().Type != HlTokenType.OpEquality;
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
            parser.Eat( parser.CurrentToken.Type );

            HlExpression token =
                new HlBinaryOp( currentNode, type, parser.ParseExpr( PrecedenceLevel ) );

            return token;
        }

        #endregion

    }

}
