﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

using VisCPU.HL.TypeSystem;

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
        public HlExpression Left { get; private set; }

        public string Instance { get; private set; }
        public HlTypeDefinition InstanceType { get; private set; }

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

        public void Redirect( HlExpression left, string instance, HlTypeDefinition instanceType )
        {
            Left = left;
            InstanceType = instanceType;
            Instance = instance;
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
