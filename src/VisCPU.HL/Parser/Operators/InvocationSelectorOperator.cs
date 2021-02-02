using System.Collections.Generic;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Parser.Operators
{

    /// <summary>
    ///     Implements Invocation Operator
    /// </summary>
    public class InvocationSelectorOperator : HlExpressionOperator
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
            return parser.CurrentToken.Type == HlTokenType.OpBracketOpen;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override HlExpression Create( HlExpressionParser parser, HlExpression currentNode )
        {
            parser.Eat( HlTokenType.OpBracketOpen );
            List < HlExpression > parameterList = new List < HlExpression >();
            bool comma = false;

            while ( parser.CurrentToken.Type != HlTokenType.OpBracketClose )
            {
                if ( comma )
                {
                    parser.Eat( HlTokenType.OpComma );
                    comma = false;
                }
                else
                {
                    parameterList.Add( parser.ParseExpr() );
                    comma = true;
                }
            }

            parser.Eat( HlTokenType.OpBracketClose );

            return new HlInvocationOp( currentNode, parameterList.ToArray() );
        }

        #endregion
    }

}
