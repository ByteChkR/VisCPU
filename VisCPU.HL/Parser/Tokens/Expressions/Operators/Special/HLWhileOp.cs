using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operators.Special
{

    /// <summary>
    ///     While Operator Implementation
    /// </summary>
    public class HlWhileOp : HlExpression
    {
        public HlExpression[] Block { get; }

        /// <summary>
        ///     Continue Expression
        /// </summary>
        public HlExpression Condition { get; }

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="condition">Continue Condition</param>
        /// <param name="operationType">Operation Type</param>
        /// <param name="exprBody">Expression Body</param>
        public HlWhileOp( HlExpression condition, HlExpression[] block, int sourceIdx ) : base( sourceIdx )
        {
            Condition = condition;
            Block = block;
        }

        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List < IHlToken > GetChildren()
        {
            return new List < IHlToken > { Condition }.Concat( Block ).ToList();
        }

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder( $"while({Condition})(" );

            foreach ( HlExpression hlExpression in Block )
            {
                ret.Append( hlExpression );
            }

            ret.Append( ')' );

            return ret.ToString();
        }

        #endregion
    }

}
