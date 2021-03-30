using System.Collections.Generic;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operators.Special
{

    /// <summary>
    ///     Member Access . Operator Implementation
    /// </summary>
    public class HlMemberAccessOp : HlExpression
    {
        /// <summary>
        ///     Left Side expression
        /// </summary>
        public HlExpression Left { get; }

        /// <summary>
        ///     Name of the Member that is beeing accessed.
        /// </summary>
        public HlExpression MemberName { get; }

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="left">Left Side Expression</param>
        /// <param name="memberName"></param>
        public HlMemberAccessOp( HlExpression left, HlExpression memberName ) : base(
            left.SourceIndex
        )
        {
            Left = left;
            MemberName = memberName;
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
            return false;
        }

        public override string ToString()
        {
            string ret = $"{Left}.{MemberName}";

            return ret;
        }

        #endregion
    }

}
