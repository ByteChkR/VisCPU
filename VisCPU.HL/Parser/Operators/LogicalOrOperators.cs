using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Parser.Operators
{

    /// <summary>
    ///     Implements Logical OR Operator
    /// </summary>
    public class LogicalOrOperators : HLExpressionOperator
    {

        /// <summary>
        ///     Precedence Level of the Operators
        /// </summary>
        public override int PrecedenceLevel => 14;

        #region Public

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public override bool CanCreate( HLExpressionParser parser, HLExpression currentNode )
        {
            return parser.CurrentToken.Type == HLTokenType.OpPipe &&
                   parser.Reader.PeekNext().Type == HLTokenType.OpPipe;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override HLExpression Create( HLExpressionParser parser, HLExpression currentNode )
        {
            parser.Eat( HLTokenType.OpPipe );
            parser.Eat( HLTokenType.OpPipe );

            return new HLBinaryOp(
                                  currentNode,
                                  HLTokenType.OpLogicalOr,
                                  parser.ParseExpr( PrecedenceLevel )
                                 );
        }

        #endregion

    }

}
