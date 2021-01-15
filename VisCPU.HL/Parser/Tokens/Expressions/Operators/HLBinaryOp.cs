using System.Collections.Generic;

/// <summary>
/// Contains XLangExpression Implementations
/// </summary>
namespace VisCPU.HL.Parser.Tokens.Expressions.Operators
{

    /// <summary>
    ///     Implements Binary Operators
    /// </summary>
    public class HLBinaryOp : HLExpression
    {

        /// <summary>
        ///     Left side of the Expression
        /// </summary>
        public readonly HLExpression Left;

        /// <summary>
        ///     The Operation Type
        /// </summary>
        public readonly HLTokenType OperationType;

        /// <summary>
        ///     Right side of the Expression
        /// </summary>
        public readonly HLExpression Right;

        public override HLTokenType Type => OperationType;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="left">Left Side</param>
        /// <param name="operationType">Operation Type</param>
        /// <param name="right">Right Side</param>
        public HLBinaryOp(
            HLExpression left,
            HLTokenType operationType,
            HLExpression right ) : base( left.SourceIndex )
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
            return new List < IHlToken >
                   {
                       Left,
                       Right
                   };
        }

        public override string ToString()
        {
            return Left + $"({OperationType})" + Right;
        }

        #endregion

    }

}
