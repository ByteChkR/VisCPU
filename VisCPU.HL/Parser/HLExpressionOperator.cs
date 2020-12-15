using VisCPU.HL.Parser.Tokens.Expressions;

namespace VisCPU.HL.Parser
{
    /// <summary>
    ///     Abstract XLangExpressionOperator
    /// </summary>
    public abstract class HLExpressionOperator
    {

        /// <summary>
        ///     Precedence Level of the Operator
        /// </summary>
        public abstract int PrecedenceLevel { get; }

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public abstract bool CanCreate(HLExpressionParser parser, HLExpression currentNode);

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public abstract HLExpression Create(HLExpressionParser parser, HLExpression currentNode);

    }
}