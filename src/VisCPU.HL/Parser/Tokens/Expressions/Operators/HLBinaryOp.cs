using System.Collections.Generic;

/// <summary>
/// Contains XLangExpression Implementations
/// </summary>
namespace VisCPU.HL.Parser.Tokens.Expressions.Operators
{

    /// <summary>
    ///     Implements Binary Operators
    /// </summary>
    public class HlBinaryOp : HlExpression
    {
        /// <summary>
        ///     Left side of the Expression
        /// </summary>
        public HlExpression Left { get; }

        /// <summary>
        ///     The Operation Type
        /// </summary>
        public HlTokenType OperationType { get; }

        /// <summary>
        ///     Right side of the Expression
        /// </summary>
        public HlExpression Right { get; }

        public override HlTokenType Type => OperationType;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="left">Left Side</param>
        /// <param name="operationType">Operation Type</param>
        /// <param name="right">Right Side</param>
        public HlBinaryOp(
            HlExpression left,
            HlTokenType operationType,
            HlExpression right ) : base( left.SourceIndex )
        {
            Left = left;
            OperationType = operationType;
            Right = right;
        }

        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List < IHlToken > GetChildren()
        {
            return new List < IHlToken > { Left, Right };
        }

        public override bool IsStatic()
        {
            return Left.IsStatic() && Right.IsStatic();
        }

        public override string ToString()
        {
            return Left + $"({OperationType})" + Right;
        }

        #endregion
    }

}
