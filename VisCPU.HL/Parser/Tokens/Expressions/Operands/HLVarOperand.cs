using System.Collections.Generic;

/// <summary>
/// Contains XLangExpression Implementations for Operand Values.
/// </summary>
namespace VisCPU.HL.Parser.Tokens.Expressions.Operands
{
    /// <summary>
    ///     Implements a Variable Operand
    /// </summary>
    public class HLVarOperand : HLExpression
    {
        #region Protected

        /// <summary>
        ///     Protected Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        protected HLVarOperand(int sourceIdx) : base(sourceIdx)
        {
        }

        #endregion

        /// <summary>
        ///     The Token Value
        /// </summary>
        public virtual IHLToken Value { get; }

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="value">Variable Value</param>
        public HLVarOperand(IHLToken value, int sourceIdx) : base(sourceIdx)
        {
            Value = value;
        }

        /// <summary>
        ///     Returns all Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List<IHLToken> GetChildren()
        {
            return new List<IHLToken> {Value};
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        #endregion
    }
}