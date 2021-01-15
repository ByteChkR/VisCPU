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
        private readonly HLExpression m_Condition;

        /// <summary>
        ///     The Expression Body
        /// </summary>
        private readonly HLExpression[] m_ExprBody;

        /// <summary>
        ///     Variable Declaration
        /// </summary>
        private readonly HLExpression m_VDecl;

        /// <summary>
        ///     Variable Change Expression
        /// </summary>
        private readonly HLExpression m_VInc;

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
            m_Condition = condition;
            m_VDecl = vDecl;
            m_VInc = vInc;
            m_ExprBody = exprBody;
        }

        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List < IHlToken > GetChildren()
        {
            return new List < IHlToken >
                   {
                       m_VDecl,
                       m_Condition,
                       m_VInc
                   }.Concat( m_ExprBody ).
                     ToList();
        }

        #endregion

    }

}
