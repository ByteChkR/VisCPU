using System.Collections.Generic;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operators
{

    /// <summary>
    ///     Implements Unary Operators
    /// </summary>
    public class HLUnaryOp : HLExpression
    {

        /// <summary>
        ///     Left Side of the Expression
        /// </summary>
        public readonly HLExpression Left;

        /// <summary>
        ///     The Expression Type
        /// </summary>
        public readonly HLTokenType OperationType;

        public override HLTokenType Type => OperationType;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="left">Left Side</param>
        /// <param name="operationType">Operation Type</param>
        public HLUnaryOp( HLExpression left, HLTokenType operationType ) : base( left.SourceIndex )
        {
            Left = left;
            OperationType = operationType;
        }

        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List < IHlToken > GetChildren()
        {
            return new List < IHlToken > { Left };
        }

        public override string ToString()
        {
            return Left + $"({OperationType})";
        }

        #endregion

    }

}
