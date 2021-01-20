using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Parser.Operators
{

    /// <summary>
    ///     Implements LessThan/GreaterThan Operators
    /// </summary>
    public class RelationOperators : HlExpressionOperator
    {
        /// <summary>
        ///     Precedence Level of the Operators
        /// </summary>
        public override int PrecedenceLevel => 8;

        #region Public

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public override bool CanCreate( HlExpressionParser parser, HlExpression currentNode )
        {
            return parser.CurrentToken.Type == HlTokenType.OpLessThan ||
                   parser.CurrentToken.Type == HlTokenType.OpGreaterThan;
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
            HlExpression node = null;

            if ( type == HlTokenType.OpLessThan )
            {
                if ( parser.CurrentToken.Type == HlTokenType.OpEquality )
                {
                    parser.Eat( HlTokenType.OpEquality );

                    node = new HlBinaryOp(
                        currentNode,
                        HlTokenType.OpLessOrEqual,
                        parser.ParseExpr( PrecedenceLevel )
                    );
                }
                else
                {
                    node = new HlBinaryOp( currentNode, type, parser.ParseExpr( PrecedenceLevel ) );
                }
            }
            else if ( type == HlTokenType.OpGreaterThan )
            {
                if ( parser.CurrentToken.Type == HlTokenType.OpEquality )
                {
                    parser.Eat( HlTokenType.OpEquality );

                    node = new HlBinaryOp(
                        currentNode,
                        HlTokenType.OpGreaterOrEqual,
                        parser.ParseExpr( PrecedenceLevel )
                    );
                }
                else
                {
                    node = new HlBinaryOp( currentNode, type, parser.ParseExpr( PrecedenceLevel ) );
                }
            }

            return node;
        }

        #endregion
    }

}
