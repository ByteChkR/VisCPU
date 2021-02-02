using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Parser.Operators
{

    /// <summary>
    ///     Implements Inequality Operator
    /// </summary>
    public class InEqualityOperators : HlExpressionOperator
    {

        /// <summary>
        ///     Precedence Level of the Operators
        /// </summary>
        public override int PrecedenceLevel => 9;

        #region Public

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public override bool CanCreate( HlExpressionParser parser, HlExpression currentNode )
        {
            return parser.CurrentToken.Type == HlTokenType.OpBang &&
                   parser.Reader.PeekNext().Type == HlTokenType.OpEquality;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override HlExpression Create( HlExpressionParser parser, HlExpression currentNode )
        {
            parser.Eat( HlTokenType.OpBang );
            parser.Eat( HlTokenType.OpEquality );

            return new HlUnaryOp(
                                 new HlBinaryOp(
                                                currentNode,
                                                HlTokenType.OpComparison,
                                                parser.ParseExpr( PrecedenceLevel )
                                               ),
                                 HlTokenType.OpBang
                                );
        }

        #endregion

    }

}
