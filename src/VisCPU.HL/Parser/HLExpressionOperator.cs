using VisCPU.HL.Parser.Tokens.Expressions;

namespace VisCPU.HL.Parser
{

    /// <summary>
    ///     Abstract XLangExpressionOperator
    /// </summary>
    public abstract class HlExpressionOperator
    {
        /// <summary>
        ///     Precedence Level of the Operator
        /// </summary>
        public abstract int PrecedenceLevel { get; }

        #region Public

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public abstract bool CanCreate( HlExpressionParser parser, HlExpression currentNode );

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public abstract HlExpression Create( HlExpressionParser parser, HlExpression currentNode );

        #endregion
    }

}
