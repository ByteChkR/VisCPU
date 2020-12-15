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
        public readonly IHLToken Value;


        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">The XL Context</param>
        /// <param name="value">The Value of this Token</param>
        public HLValueOperand(IHLToken value) : base(value.SourceIndex)
        {
            Value = value;
        }

        /// <summary>
        ///     Returns all Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List<IHLToken> GetChildren()
        {
            return new List<IHLToken> { Value };
        }

        public override string ToString()
        {
            return Value.ToString();
        }

    }
}