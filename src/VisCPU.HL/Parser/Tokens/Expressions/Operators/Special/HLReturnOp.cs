using System.Collections.Generic;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operators.Special
{

    /// <summary>
    ///     Return Operator Implementation
    /// </summary>
    public class HlReturnOp : HlExpression
    {

        /// <summary>
        ///     Right side expression (return value)
        /// </summary>
        public HlExpression Right { get; }

        public override HlTokenType Type => HlTokenType.OpReturn;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="right">Right side Expression</param>
        public HlReturnOp( HlExpression right, int sourceIdx ) : base( sourceIdx )
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

        public override bool IsStatic()
        {
            return Right.IsStatic();
        }

        public override string ToString()
        {
            return $"return {Right}";
        }

        #endregion

    }

}
