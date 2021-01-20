using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operators.Special
{

    /// <summary>
    ///     Invocation () Operator Implementation
    /// </summary>
    public class HlInvocationOp : HlExpression
    {
        /// <summary>
        ///     Left side Expression
        /// </summary>
        public HlExpression Left { get; }

        /// <summary>
        ///     Invocation Arguments
        /// </summary>
        public HlExpression[] ParameterList { get; }

        /// <summary>
        ///     Operation Type
        /// </summary>
        public override HlTokenType Type => HlTokenType.OpInvocation;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="left">Left Side Expression</param>
        /// <param name="parameterList">Parameter list</param>
        public HlInvocationOp( HlExpression left, HlExpression[] parameterList ) : base( left.SourceIndex )
        {
            Left = left;
            ParameterList = parameterList;
        }

        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List < IHlToken > GetChildren()
        {
            return ParameterList.Cast < IHlToken >().Concat( new[] { Left } ).ToList();
        }

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder( $"{Left}(" );

            for ( int i = 0; i < ParameterList.Length; i++ )
            {
                HlExpression hlExpression = ParameterList[i];
                ret.Append( hlExpression );

                if ( i != ParameterList.Length - 1 )
                {
                    ret.Append( ',' );
                }
            }

            ret.Append( ')' );

            return ret.ToString();
        }

        #endregion
    }

}
