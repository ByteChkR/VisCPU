using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Math.Assignments
{

    public class DivAssignExpressionCompiler : SelfAssignExpressionCompiler
    {
        protected override string InstructionKey => "DIV";

        #region Public

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
            HlBinaryOp expr )
        {
            ExpressionTarget target = compilation.Parse( expr.Left );

            ExpressionTarget rTarget = compilation.Parse(
                expr.Right
            );

            if ( SettingsManager.GetSettings < HlCompilerSettings >().OptimizeReduceExpressions &&
                 IsReducable( rTarget ) )
            {
                uint amount = GetPowLevel( rTarget.StaticParse() );
                string tmp = compilation.GetTempVarLoad( amount.ToString() );

                compilation.EmitterResult.Emit(
                    "SHR",
                    target.MakeAddress( compilation ).ResultAddress,
                    tmp
                );

                compilation.ReleaseTempVar( tmp );

                return target;
            }

            rTarget = rTarget.MakeAddress( compilation );

            string instrKey =
                target.TypeDefinition.Name == HLBaseTypeNames.s_FloatTypeName ||
                rTarget.TypeDefinition.Name == HLBaseTypeNames.s_FloatTypeName
                    ? InstructionKey + ".F"
                    : InstructionKey;

            compilation.EmitterResult.Emit(
                instrKey,
                target.ResultAddress,
                rTarget.ResultAddress
            );

            compilation.ReleaseTempVar( rTarget.ResultAddress );

            return target;
        }

        #endregion

        #region Private

        private uint GetPowLevel( uint num )
        {
            uint count = 1;
            uint current = num;

            while ( ( current >>= 1 ) != 1 )
            {
                count++;
            }

            return count;
        }

        private bool IsReducable( ExpressionTarget target )
        {
            if ( !target.IsAddress )
            {
                uint v = target.StaticParse();

                return v != 0 && ( v & ( v - 1 ) ) == 0; //Is Power of 2?
            }

            return false;
        }

        #endregion
    }

}
