using System.Collections.Generic;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

/// <summary>
/// Contains Operator Implementations for the XLangExpressionParser
/// </summary>
namespace VisCPU.HL.Parser.Operators
{

    /// <summary>
    ///     Implements ArraySubscript Operator
    /// </summary>
    public class ArraySubscriptOperator : HLExpressionOperator
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
            return parser.CurrentToken.Type == HLTokenType.OpIndexerBracketOpen;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override HLExpression Create( HLExpressionParser parser, HLExpression currentNode )
        {
            parser.Eat( HLTokenType.OpIndexerBracketOpen );
            List < HLExpression > parameterList = new List < HLExpression >();
            bool comma = false;

            while ( parser.CurrentToken.Type != HLTokenType.OpIndexerBracketClose )
            {
                if ( comma )
                {
                    parser.Eat( HLTokenType.OpComma );
                }
                else
                {
                    parameterList.Add( parser.ParseExpr() );
                    comma = true;
                }
            }

            parser.Eat( HLTokenType.OpIndexerBracketClose );

            return new HLArrayAccessorOp( currentNode, parameterList );
        }

        #endregion
    }

}
