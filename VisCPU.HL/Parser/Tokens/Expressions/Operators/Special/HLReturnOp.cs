using System.Collections.Generic;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operators.Special
{

    /// <summary>
    ///     Return Operator Implementation
    /// </summary>
    public class HLReturnOp : HLExpression
    {
        /// <summary>
        ///     Right side expression (return value)
        /// </summary>
        public HLExpression Right { get; }

        public override HLTokenType Type => HLTokenType.OpReturn;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="right">Right side Expression</param>
        public HLReturnOp( HLExpression right, int sourceIdx ) : base( sourceIdx )
        {
            Right = right;
        }

        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List < IHlToken > GetChildren()
        {
            return new List < IHlToken > { Right };
        }

        public override string ToString()
        {
            return $"return {Right}";
        }

        #endregion
    }

}
