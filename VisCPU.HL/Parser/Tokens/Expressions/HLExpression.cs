using System.Collections.Generic;

/// <summary>
/// Contains XLangExpressionParser Token Implementations
/// </summary>
namespace VisCPU.HL.Parser.Tokens.Expressions
{
    /// <summary>
    ///     Implements the base of any XLangExpression implementation
    /// </summary>
    public abstract class HLExpression : IHLToken
    {
        #region Protected

        /// <summary>
        ///     Protected Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        protected HLExpression(int sourceIndex)
        {
            SourceIndex = sourceIndex;
        }

        #endregion

        /// <summary>
        ///     Start index in source
        /// </summary>
        public int SourceIndex { get; }

        /// <summary>
        ///     The Token Type (OpExpression)
        /// </summary>
        public virtual HLTokenType Type => HLTokenType.OpExpression;

        #region Public

        /// <summary>
        ///     Returns the Child Tokens of this token
        /// </summary>
        /// <returns></returns>
        public abstract List<IHLToken> GetChildren();

        #endregion
    }
}