using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Parser.Operators
{

    /// <summary>
    ///     Implements Assignment Operator
    /// </summary>
    public class AssignmentOperators : HlExpressionOperator
    {

        /// <summary>
        ///     Precedence Level of the Operators
        /// </summary>
        public override int PrecedenceLevel => 15;

        #region Public

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public override bool CanCreate( HlExpressionParser parser, HlExpression currentNode )
        {
            return parser.CurrentToken.Type == HlTokenType.OpEquality &&
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
            parser.Eat( HlTokenType.OpEquality );

            return new HlBinaryOp( currentNode, HlTokenType.OpEquality, parser.ParseExpr( PrecedenceLevel ) );
        }

        #endregion

    }

}
