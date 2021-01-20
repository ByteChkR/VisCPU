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
    public class HLArrayAccessorOp : HLExpression
    {
        /// <summary>
        ///     Left Side (the array)
        /// </summary>
        public HLExpression Left { get; }

        /// <summary>
        ///     The Accessor Arguments
        /// </summary>
        public HLExpression[] ParameterList { get; }

        /// <summary>
        ///     The Operator Token
        /// </summary>
        public override HLTokenType Type => HLTokenType.OpArrayAccess;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="list">Left Side Array</param>
        /// <param name="parameterList">Array Accessor Parameters</param>
        public HLArrayAccessorOp( HLExpression list, List < HLExpression > parameterList ) : base( list.SourceIndex )
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

        #endregion
    }

}
