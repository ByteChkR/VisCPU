﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

using VisCPU.HL.DataTypes;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility.SharedBase;

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

        public bool WriteProlog { get; private set; }

        public HlTypeDefinition InstanceType { get; private set; }

        public HlMemberDefinition MemberDefinition { get; private set; }

        /// <summary>
        ///     Invocation Arguments
        /// </summary>
        public HlExpression[] ParameterList { get; }

        /// <summary>
        ///     Operation FunctionType
        /// </summary>
        public override HlTokenType Type => HlTokenType.OpInvocation;

        public override HlTypeDefinition GetResultType( HlCompilation c )
        {
            return MemberDefinition is HlFunctionDefinition fdef
                       ? fdef.ReturnType
                       : c.ExternalSymbols.Any( x => x.GetName() == Left.ToString() && x is FunctionData )
                           ? c.TypeSystem.GetType(
                                                  c.Root,
                                                  ( c.ExternalSymbols.First( x => x.GetName() == Left.ToString() )
                                                        as FunctionData ).ReturnType
                                                 )
                           : c.TypeSystem.GetType( c.Root, HLBaseTypeNames.s_UintTypeName );
        }

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

        public override bool IsStatic()
        {
            return false;
        }

        public void Redirect(
            string instance,
            HlTypeDefinition instanceType,
            HlMemberDefinition memberDef,
            bool writeProlog = true )
        {
            WriteProlog = writeProlog;
            InstanceType = instanceType;
            MemberDefinition = memberDef;

            if ( memberDef != null )
            {
                Left = new HlValueOperand(
                                          new HlTextToken(
                                                          HlTokenType.OpWord,
                                                          InstanceType.GetFinalMemberName(
                                                               memberDef
                                                              ), //$"FUN_{tdef.Name}_{tdef.Constructor.Name}",
                                                          0
                                                         )
                                         );
            }

            Instance = instance;
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
