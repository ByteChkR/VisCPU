using System.Collections.Generic;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operands
{

    /// <summary>
    ///     Implements a (terminal) value expression
    /// </summary>
    public class HLValueOperand : HLExpression
    {

        /// <summary>
        ///     The Value
        /// </summary>
        public readonly IHlToken Value;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">The XL Context</param>
        /// <param name="value">The Value of this Token</param>
        public HLValueOperand( IHlToken value ) : base( value.SourceIndex )
        {
            Value = value;
        }

        /// <summary>
        ///     Returns all Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List < IHlToken > GetChildren()
        {
            return new List < IHlToken > { Value };
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        #endregion

    }

}
