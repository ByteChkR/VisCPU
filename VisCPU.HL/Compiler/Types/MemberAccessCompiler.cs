using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.HL.TypeSystem;

namespace VisCPU.HL.Compiler.Types
{

    public class MemberAccessCompiler : HLExpressionCompiler < HLMemberAccessOp >
    {

        protected override bool AllImplementations => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLMemberAccessOp expr,
            ExpressionTarget outputTarget )
        {
            string tmpVar;
            ExpressionTarget lType = compilation.Parse( expr.Left );

            uint off = HLTypeDefinition.RecursiveGetOffset(
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

            compilation.ProgramCode.Add( $"ADD {tmpVar} {tmpOff}" );

            compilation.ReleaseTempVar( tmpOff );

            if ( outputTarget.ResultAddress != null )
            {
                compilation.ProgramCode.Add( $"DREF {tmpVar} {outputTarget.ResultAddress}" );
                compilation.ReleaseTempVar( tmpVar );

                return outputTarget;
            }

            HLMemberDefinition mdef = HLTypeDefinition.RecursiveGetPublicMember( lType.TypeDefinition,0, expr.MemberName.ToString().Split('.'));

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
