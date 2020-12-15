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
        public readonly HLExpression Left;

        /// <summary>
        ///     The Accessor Arguments
        /// </summary>
        public readonly HLExpression[] ParameterList;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="list">Left Side Array</param>
        /// <param name="parameterList">Array Accessor Parameters</param>
        public HLArrayAccessorOp(HLExpression list, List<HLExpression> parameterList) : base(list.SourceIndex)
        {
            Left = list;
            ParameterList = parameterList.ToArray();
        }

        /// <summary>
        ///     The Operator Token
        /// </summary>
        public override HLTokenType Type => HLTokenType.OpArrayAccess;


        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List<IHLToken> GetChildren()
        {
            return ParameterList.Cast<IHLToken>().Concat(new[] { Left }).ToList();
        }

    }
}