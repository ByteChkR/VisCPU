using System;
using System.Diagnostics;

using VisCPU.HL.DataTypes;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Types
{

    public class MemberAccessCompiler : HlExpressionCompiler < HlMemberAccessOp >
    {

        protected override bool AllImplementations => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
            HlMemberAccessOp expr,
            ExpressionTarget outputTarget )
        {
            string tmpVar;
            ExpressionTarget lType = compilation.Parse( expr.Left );

            if ( lType.ResultAddress == "%%TYPE%%" )
            {
                if (expr.MemberName is HlInvocationOp invoc)
                {

                    HlMemberDefinition data = lType.TypeDefinition.GetPrivateOrPublicMember(invoc.Left.ToString());

                    string funcName = lType.TypeDefinition.GetFinalMemberName( data );//$"FUN_{lType.TypeDefinition.Name}_{invoc.Left}";


                    if ( !data.IsStatic )
                    {
                        Log( "AAAAAAA" );
                    }
                    invoc.Redirect( null , lType.TypeDefinition, data);

                    ExpressionTarget t = compilation.Parse( invoc, outputTarget ).
                                                     CopyIfNotNull( compilation, outputTarget );



                    return t;
                }
                else
                {
                    HlMemberDefinition data = lType.TypeDefinition.GetPrivateOrPublicMember(expr.MemberName.ToString());
                    string funcName = lType.TypeDefinition.GetFinalMemberName(data);//$"FUN_{lType.TypeDefinition.Name}_{expr.MemberName}";

                    if (!data.IsStatic)
                    {
                        Log("AAAAAAA");
                    }
                    if ( outputTarget.ResultAddress != null )
                    {
                        compilation.EmitterResult.Emit("LOAD", funcName, outputTarget.ResultAddress);
                        return outputTarget;
                    }

                    return new ExpressionTarget(
                                                funcName,
                                                true, compilation.TypeSystem.GetType(HLBaseTypeNames.s_UintTypeName), false
                                               );
                }
            }

            string containerName = expr.MemberName.ToString();
            if ( expr.MemberName is HlInvocationOp inv )
            {
                if(lType.TypeDefinition.HasMember(inv.Left.ToString()) && lType.TypeDefinition.GetPrivateOrPublicMember(inv.Left.ToString()) is HlPropertyDefinition)
                {
                    containerName = inv.Left.ToString();
                }
                else
                {

                    HlMemberDefinition data = lType.TypeDefinition.GetPrivateOrPublicMember(inv.Left.ToString());

                    inv.Redirect(
                                 lType.ResultAddress,
                                 lType.TypeDefinition,
                                 data
                                );

                    ExpressionTarget t = compilation.Parse( inv, outputTarget ).
                                                     CopyIfNotNull( compilation, outputTarget );

                    return t;
                }

            }
            else if ( expr.MemberName is HlArrayAccessorOp arr && lType.TypeDefinition.GetPrivateOrPublicMember(arr.Left.ToString()) is HlPropertyDefinition)
            {

                containerName = arr.Left.ToString();

            }



            uint off = HlTypeDefinition.RecursiveGetOffset(
                                                           lType.TypeDefinition,
                                                           0,
                                                           0,
                                                           containerName.Split( '.' )
                                                          );

            string tmpOff = compilation.GetTempVar( off );

            if ( lType.IsPointer )
            {
                tmpVar = compilation.GetTempVarCopy( lType.ResultAddress );
            }
            else
            {
                tmpVar = compilation.GetTempVarLoad( lType.ResultAddress );
            }

            compilation.EmitterResult.Emit( $"ADD", tmpVar, tmpOff );

            compilation.ReleaseTempVar( tmpOff );

            if ( outputTarget.ResultAddress != null )
            {
                compilation.EmitterResult.Emit( $"DREF", tmpVar, outputTarget.ResultAddress );
                compilation.ReleaseTempVar( tmpVar );

                return outputTarget;
            }

            HlMemberDefinition mdef = null;

            if ( expr.Left.ToString() == "this" )
            {
                mdef = HlTypeDefinition.RecursiveGetPrivateOrPublicMember(
                                                                 lType.TypeDefinition,
                                                                 0,
                                                                 containerName.Split('.')
                                                                );

            }
            else
            {

            mdef  =  HlTypeDefinition.RecursiveGetPublicMember(
                                                          lType.TypeDefinition,
                                                          0,
                                                          containerName.Split('.')
                                                         );

            }

            if ( mdef is HlPropertyDefinition pdef )
            {
                return new ExpressionTarget( tmpVar, true, pdef.PropertyType, true );
            }

            if ( mdef is HlFunctionDefinition fdef )
            {
                return new ExpressionTarget( tmpVar, true, fdef.ReturnType, true );
            }

            return new ExpressionTarget();
        }

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlMemberAccessOp expr )
        {
            return ParseExpression( compilation, expr, new ExpressionTarget() );
        }

        #endregion

    }

}
