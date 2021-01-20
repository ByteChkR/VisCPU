using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Parser.Operators
{

    /// <summary>
    ///     Implements XOR Operator
    /// </summary>
    public class BitXOrOperators : HLExpressionOperator
    {
        /// <summary>
        ///     Precedence Level of the Operators
        /// </summary>
        public override int PrecedenceLevel => 11;

        #region Public

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public override bool CanCreate( HLExpressionParser parser, HLExpression currentNode )
        {
            return parser.CurrentToken.Type == HLTokenType.OpCap &&
                   parser.Reader.PeekNext().Type != HLTokenType.OpEquality;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override HLExpression Create( HLExpressionParser parser, HLExpression currentNode )
        {
            parser.Eat( HLTokenType.OpCap );

            return new HLBinaryOp(
                currentNode,
                HLTokenType.OpCap,
                parser.ParseExpr( PrecedenceLevel )
            );
        }

        #endregion
    }

}
