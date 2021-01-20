using System.Collections.Generic;

/// <summary>
/// Contains XLangExpressionParser Token Implementations
/// </summary>
namespace VisCPU.HL.Parser.Tokens.Expressions
{

    /// <summary>
    ///     Implements the base of any XLangExpression implementation
    /// </summary>
    public abstract class HlExpression : IHlToken
    {
        /// <summary>
        ///     Start index in source
        /// </summary>
        public int SourceIndex { get; }

        /// <summary>
        ///     The Token Type (OpExpression)
        /// </summary>
        public virtual HlTokenType Type => HlTokenType.OpExpression;

        #region Public

        /// <summary>
        ///     Returns the Child Tokens of this token
        /// </summary>
        /// <returns></returns>
        public abstract List < IHlToken > GetChildren();

        #endregion

        #region Protected

        /// <summary>
        ///     Protected Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        protected HlExpression( int sourceIndex )
        {
            SourceIndex = sourceIndex;
        }

        #endregion
    }

}
