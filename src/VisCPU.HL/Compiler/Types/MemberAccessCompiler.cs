using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.HL.TypeSystem;

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

            uint off = HlTypeDefinition.RecursiveGetOffset(
                lType.TypeDefinition,
                0,
                0,
                expr.MemberName.ToString().Split( '.' )
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

            HlMemberDefinition mdef =
                HlTypeDefinition.RecursiveGetPublicMember(
                    lType.TypeDefinition,
                    0,
                    expr.MemberName.ToString().Split( '.' )
                );

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
