﻿using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.HL.TypeSystem;

namespace VisCPU.HL.Compiler
{

    public class MemberAccessCompiler : HLExpressionCompiler < HLMemberAccessOp >
    {

        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLMemberAccessOp expr,
            ExpressionTarget outputTarget )
        {
            string tmpVar;
            ExpressionTarget lType = compilation.Parse( expr.Left );
            uint off = lType.TypeDefinition.GetOffset( expr.MemberName );
            string tmpOff = compilation.GetTempVar(off);

            if ( lType.IsPointer )
            {
                tmpVar = lType.ResultAddress;
            }
            else
            {
                tmpVar = compilation.GetTempVarLoad(lType.ResultAddress);
            }
            
            compilation.ProgramCode.Add( $"ADD {tmpVar} {tmpOff}" );

            compilation.ReleaseTempVar(tmpOff);
            if ( outputTarget.ResultAddress != null )
            {
                compilation.ProgramCode.Add( $"DREF {tmpVar} {outputTarget.ResultAddress}" );
                compilation.ReleaseTempVar( tmpVar );

                return outputTarget;
            }

            HLMemberDefinition mdef = lType.TypeDefinition.GetMember( expr.MemberName );

            if ( mdef is HLPropertyDefinition pdef )
            {
                return new ExpressionTarget( tmpVar, true, pdef.PropertyType, true );
            }

            if ( mdef is HLFunctionDefinition fdef )
            {
                return new ExpressionTarget( tmpVar, true, fdef.ReturnType, true );
            }

            return new ExpressionTarget();
        }

        public override ExpressionTarget ParseExpression( HLCompilation compilation, HLMemberAccessOp expr )
        {
            return ParseExpression( compilation, expr, new ExpressionTarget() );
        }

        #endregion

    }

}
