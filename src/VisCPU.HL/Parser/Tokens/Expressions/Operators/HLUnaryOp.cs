using System.Collections.Generic;

using VisCPU.HL.TypeSystem;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operators
{

    /// <summary>
    ///     Implements Unary Operators
    /// </summary>
    public class HlUnaryOp : HlExpression
    {

        /// <summary>
        ///     Left Side of the Expression
        /// </summary>
        public HlExpression Left { get; }

        /// <summary>
        ///     The Expression FunctionType
        /// </summary>
        public HlTokenType OperationType { get; }

        public override HlTokenType Type => OperationType;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="left">Left Side</param>
        /// <param name="operationType">Operation FunctionType</param>
        public HlUnaryOp( HlExpression left, HlTokenType operationType ) : base( left.SourceIndex )
        {
            Left = left;
            OperationType = operationType;
        }

        public override HlTypeDefinition GetResultType(HlCompilation c)
        {
            return Left.GetResultType(c);
        }

        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List < IHlToken > GetChildren()
        {
            return new List < IHlToken > { Left };
        }

        public override bool IsStatic()
        {
            return Left.IsStatic();
        }

        public override string ToString()
        {
            return Left + $"({Type})";
        }

        #endregion

    }

}
