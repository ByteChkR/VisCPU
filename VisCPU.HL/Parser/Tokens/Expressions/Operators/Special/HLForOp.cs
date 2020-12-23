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
        ///     Continue Condition
        /// </summary>
        private readonly HLExpression Condition;

        /// <summary>
        ///     The Expression Body
        /// </summary>
        private readonly HLExpression[] ExprBody;

        /// <summary>
        ///     Variable Declaration
        /// </summary>
        private readonly HLExpression VDecl;

        /// <summary>
        ///     Variable Change Expression
        /// </summary>
        private readonly HLExpression VInc;

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
            int sourceIdx) : base(sourceIdx)
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
        public override List<IHLToken> GetChildren()
        {
            return new List<IHLToken>
            {
                VDecl,
                Condition,
                VInc
            }.Concat(ExprBody).ToList();
        }

        #endregion
    }
}