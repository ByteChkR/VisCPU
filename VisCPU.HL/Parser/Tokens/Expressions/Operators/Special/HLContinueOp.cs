using System.Collections.Generic;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operators.Special
{

    /// <summary>
    ///     Continue Operator Implementation
    /// </summary>
    public class HLContinueOp : HLExpression
    {

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        public HLContinueOp( int sourceIdx ) : base( sourceIdx )
        {
        }

        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List < IHLToken > GetChildren()
        {
            return new List < IHLToken >();
        }

        #endregion

    }

}
