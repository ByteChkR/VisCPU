using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Contains Special XLangExpression Implementations
/// </summary>
namespace VisCPU.HL.Parser.Tokens.Expressions.Operators.Special
{

    /// <summary>
    ///     Array Accessor Operator Implementation
    /// </summary>
    public class HlArrayAccessorOp : HlExpression
    {

        /// <summary>
        ///     Left Side (the array)
        /// </summary>
        public HlExpression Left { get; }

        /// <summary>
        ///     The Accessor Arguments
        /// </summary>
        public HlExpression[] ParameterList { get; }

        /// <summary>
        ///     The Operator Token
        /// </summary>
        public override HlTokenType Type => HlTokenType.OpArrayAccess;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="list">Left Side Array</param>
        /// <param name="parameterList">Array Accessor Parameters</param>
        public HlArrayAccessorOp( HlExpression list, List < HlExpression > parameterList ) : base( list.SourceIndex )
        {
            Left = list;
            ParameterList = parameterList.ToArray();
        }

        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List < IHlToken > GetChildren()
        {
            return ParameterList.Cast < IHlToken >().Concat( new[] { Left } ).ToList();
        }

        public override bool IsStatic()
        {
            return false;
        }

        #endregion

    }

}
