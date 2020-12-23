using System.Collections.Generic;
using System.Linq;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operators.Special
{
    /// <summary>
    ///     Invocation () Operator Implementation
    /// </summary>
    public class HLInvocationOp : HLExpression
    {
        /// <summary>
        ///     Left side Expression
        /// </summary>
        public readonly HLExpression Left;

        /// <summary>
        ///     Invocation Arguments
        /// </summary>
        public readonly HLExpression[] ParameterList;

        /// <summary>
        ///     Operation Type
        /// </summary>
        public override HLTokenType Type => HLTokenType.OpInvocation;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="left">Left Side Expression</param>
        /// <param name="parameterList">Parameter list</param>
        public HLInvocationOp(HLExpression left, HLExpression[] parameterList) : base(left.SourceIndex)
        {
            Left = left;
            ParameterList = parameterList;
        }

        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List<IHLToken> GetChildren()
        {
            return ParameterList.Cast<IHLToken>().Concat(new[] {Left}).ToList();
        }

        public override string ToString()
        {
            string ret = $"{Left}(";

            for (int i = 0; i < ParameterList.Length; i++)
            {
                HLExpression hlExpression = ParameterList[i];
                ret += hlExpression;

                if (i != ParameterList.Length - 1)
                {
                    ret += ",";
                }
            }

            ret += ")";

            return ret;
        }

        #endregion
    }
}