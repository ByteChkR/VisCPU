using System.Collections.Generic;
using System.Linq;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operators.Special
{

    /// <summary>
    ///     For Operator Implementation
    /// </summary>
    public class HLForOp : HLExpression
    {
        /// <summary>
        ///     Variable Change Expression
        /// </summary>
        public HLExpression VInc { get; }

        /// <summary>
        ///     Continue Condition
        /// </summary>
        public HLExpression Condition { get; }

        /// <summary>
        ///     The Expression Body
        /// </summary>
        public HLExpression[] ExprBody { get; }

        /// <summary>
        ///     Variable Declaration
        /// </summary>
        public HLExpression VDecl { get; }

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="vDecl">Variable Declaration</param>
        /// <param name="condition">For Continue Condition</param>
        /// <param name="vInc">Variable Change Expression</param>
        /// <param name="operationType">Operation Type</param>
        /// <param name="exprBody">The Expression Body</param>
        public HLForOp(
            HLExpression vDecl,
            HLExpression condition,
            HLExpression vInc,
            HLExpression[] exprBody,
            int sourceIdx ) : base( sourceIdx )
        {
            Condition = condition;
            VDecl = vDecl;
            VInc = vInc;
            ExprBody = exprBody;
        }

        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List < IHlToken > GetChildren()
        {
            return new List < IHlToken > { VDecl, Condition, VInc }.Concat( ExprBody ).
                                                                    ToList();
        }

        #endregion
    }

}
