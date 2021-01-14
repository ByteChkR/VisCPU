using System.Collections.Generic;

using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Parser.Operators
{

    /// <summary>
    ///     Implements Invocation Operator
    /// </summary>
    public class InvocationSelectorOperator : HLExpressionOperator
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
            return parser.CurrentToken.Type == HLTokenType.OpBracketOpen;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override HLExpression Create( HLExpressionParser parser, HLExpression currentNode )
        {
            parser.Eat( HLTokenType.OpBracketOpen );
            List < HLExpression > parameterList = new List < HLExpression >();
            bool comma = false;

            while ( parser.CurrentToken.Type != HLTokenType.OpBracketClose )
            {
                if ( comma )
                {
                    parser.Eat( HLTokenType.OpComma );
                    comma = false;
                }
                else
                {
                    parameterList.Add( parser.ParseExpr() );
                    comma = true;
                }
            }

            parser.Eat( HLTokenType.OpBracketClose );

            return new HLInvocationOp( currentNode, parameterList.ToArray() );
        }

        #endregion

    }

}
