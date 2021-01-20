using System.Collections.Generic;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operators.Special
{

    /// <summary>
    ///     Member Access . Operator Implementation
    /// </summary>
    public class HLMemberAccessOp : HLExpression
    {
        /// <summary>
        ///     Left Side expression
        /// </summary>
        public HLExpression Left { get; }

        /// <summary>
        ///     Name of the Member that is beeing accessed.
        /// </summary>
        public HLExpression MemberName { get; }

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="left">Left Side Expression</param>
        /// <param name="memberName"></param>
        public HLMemberAccessOp( HLExpression left, HLExpression memberName ) : base(
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

        public override string ToString()
        {
            string ret = $"{Left}.{MemberName}";

            return ret;
        }

        #endregion
    }

}
