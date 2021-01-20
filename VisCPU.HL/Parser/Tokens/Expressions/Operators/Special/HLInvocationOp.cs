using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public HLExpression Left { get; }

        /// <summary>
        ///     Invocation Arguments
        /// </summary>
        public HLExpression[] ParameterList { get; }

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
        public HLInvocationOp( HLExpression left, HLExpression[] parameterList ) : base( left.SourceIndex )
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
                HLExpression hlExpression = ParameterList[i];
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
