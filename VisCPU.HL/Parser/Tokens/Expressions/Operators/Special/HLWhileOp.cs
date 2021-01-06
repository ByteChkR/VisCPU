using System.Collections.Generic;
using System.Linq;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operators.Special
{

    /// <summary>
    ///     While Operator Implementation
    /// </summary>
    public class HLWhileOp : HLExpression
    {

        public readonly HLExpression[] Block;

        /// <summary>
        ///     Continue Expression
        /// </summary>
        public readonly HLExpression Condition;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="condition">Continue Condition</param>
        /// <param name="operationType">Operation Type</param>
        /// <param name="exprBody">Expression Body</param>
        public HLWhileOp( HLExpression condition, HLExpression[] block, int sourceIdx ) : base( sourceIdx )
        {
            Condition = condition;
            Block = block;
        }

        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List < IHLToken > GetChildren()
        {
            return new List < IHLToken > { Condition }.Concat( Block ).ToList();
        }

        public override string ToString()
        {
            string ret = $"while({Condition})(";

            foreach ( HLExpression xLangExpression in Block )
            {
                ret += xLangExpression;
            }

            ret += ")";

            return ret;
        }

        #endregion

    }

}
