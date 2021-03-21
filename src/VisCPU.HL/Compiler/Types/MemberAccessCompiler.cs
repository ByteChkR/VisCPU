using System;

using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Types
{

    public class StaticInstanceMemberAccessEvent:WarningEvent
    {

        public StaticInstanceMemberAccessEvent( HlTypeDefinition type, HlMemberDefinition member) : base( $"Accessing Instance Function '{member.Name}' in type '{type.Namespace.FullName}::{type.Name}' as static function. Passing an instance of '{type.Namespace.FullName}::{type.Name}' is required", WarningEventKeys.s_StaticInstanceMemberAccess )
        {
        }

    }

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
                if ( expr.MemberName is HlInvocationOp invoc )
                {
                    HlMemberDefinition data = null;
                    bool writeProlog = invoc.Left.ToString() == "new";
                    if ( invoc.Left.ToString() == "new" || invoc.Left.ToString() == "base")
                    {
                        data = lType.TypeDefinition.StaticConstructor;
                    }else
                        data= lType.TypeDefinition.GetPrivateOrPublicMember( invoc.Left.ToString() );


                    if (lType.TypeDefinition.IsAbstract&& writeProlog && data == lType.TypeDefinition.StaticConstructor )
                    {
                        EventManager<ErrorEvent>.SendEvent(
                                                             new AbstractConstructorCallEvent(lType.TypeDefinition)
                                                            );
                    }

                    if ( data!=null&& !data.IsStatic )
                    {
                        EventManager < WarningEvent >.SendEvent(
                                                                new StaticInstanceMemberAccessEvent(
                                                                     lType.TypeDefinition,
                                                                     data
                                                                    )
                                                               );
                    }


                    invoc.Redirect( null, lType.TypeDefinition, data , writeProlog);

                    ExpressionTarget t = compilation.Parse( invoc, outputTarget ).
                                                     CopyIfNotNull( compilation, outputTarget );

                    return t;
                }
                else
                {
                    HlMemberDefinition data =
                        lType.TypeDefinition.GetPrivateOrPublicMember( expr.MemberName.ToString() );

                    string funcName =
                        lType.TypeDefinition.
                              GetFinalMemberName( data ); //$"FUN_{lType.TypeDefinition.Name}_{expr.MemberName}";

                    if ( data.IsStatic && data is HlPropertyDefinition propDef )
                    {
                        funcName = compilation.GetFinalName( funcName );
                    }

                    if ( !data.IsStatic )
                    {
                        EventManager<WarningEvent>.SendEvent(
                                                             new StaticInstanceMemberAccessEvent(
                                                                  lType.TypeDefinition,
                                                                  data
                                                                 )
                                                            );
                    }

                    if ( outputTarget.ResultAddress != null )
                    {
                        compilation.EmitterResult.Emit( "LOAD", funcName, outputTarget.ResultAddress );

                        return outputTarget;
                    }

                    return new ExpressionTarget(
                                                funcName,
                                                true,
                                                compilation.TypeSystem.GetType(compilation.Root, HLBaseTypeNames.s_UintTypeName ),
                                                false
                                               );
                }
            }

            string containerName = expr.MemberName.ToString();

            if ( expr.MemberName is HlInvocationOp inv )
            {
                if ( lType.TypeDefinition.HasMember( inv.Left.ToString() ) &&
                     lType.TypeDefinition.GetPrivateOrPublicMember( inv.Left.ToString() ) is HlPropertyDefinition )
                {
                    containerName = inv.Left.ToString();
                }
                else
                {
                    HlMemberDefinition data = lType.TypeDefinition.GetPrivateOrPublicMember( inv.Left.ToString() );

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
            else if ( expr.MemberName is HlArrayAccessorOp arr &&
                      lType.TypeDefinition.GetPrivateOrPublicMember( arr.Left.ToString() ) is HlPropertyDefinition )
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

            

            HlMemberDefinition mdef = null;

            if ( expr.Left.ToString() == "this" )
            {
                mdef = HlTypeDefinition.RecursiveGetPrivateOrPublicMember(
                                                                          lType.TypeDefinition,
                                                                          0,
                                                                          containerName.Split( '.' )
                                                                         );
            }
            else
            {
                mdef = HlTypeDefinition.RecursiveGetPublicMember(
                                                                 lType.TypeDefinition,
                                                                 0,
                                                                 containerName.Split( '.' )
                                                                );
            }

            if ( mdef is HlPropertyDefinition pdef )
            {
                if (outputTarget.ResultAddress != null)
                {
                    compilation.EmitterResult.Emit($"DREF", tmpVar, outputTarget.ResultAddress);
                    compilation.ReleaseTempVar(tmpVar);

                    return outputTarget;
                }
                return new ExpressionTarget( tmpVar, true, pdef.PropertyType, true );
            }

            if ( mdef is HlFunctionDefinition fdef )
            {
                if (outputTarget.ResultAddress != null)
                {
                    
                    compilation.EmitterResult.Emit($"DREF", tmpVar, outputTarget.ResultAddress);
                    compilation.ReleaseTempVar(tmpVar);

                    return outputTarget;
                }
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
