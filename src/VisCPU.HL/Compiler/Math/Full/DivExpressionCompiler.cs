using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Math.Full
{

    public class DivExpressionCompiler : MathExpressionCompiler
    {
        protected override string InstructionKey => "DIV";

        #region Public

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
            HlBinaryOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget target = compilation.Parse( expr.Left );
            ExpressionTarget rTarget = compilation.Parse( expr.Right );

            if ( SettingsManager.GetSettings < HlCompilerSettings >().OptimizeConstExpressions &&
                 !target.IsAddress &&
                 !rTarget.IsAddress )
            {
                return ComputeStatic( compilation, target, rTarget );
            }

            if ( SettingsManager.GetSettings < HlCompilerSettings >().OptimizeReduceExpressions )
            {
                uint amount = 0;
                ExpressionTarget baseExpr = default;

                if ( IsReducable( rTarget ) )
                {
                    baseExpr = target.MakeAddress( compilation );
                    amount = GetPowLevel( rTarget.StaticParse() );
                }

                if ( amount != 0 )
                {
                    string tmp = compilation.GetTempVarLoad( amount.ToString() );

                    compilation.EmitterResult.Emit(
                        "SHR",
                        baseExpr.ResultAddress,
                        tmp,
                        outputTarget.ResultAddress
                    );

                    compilation.ReleaseTempVar( tmp );

                    return outputTarget;
                }
            }

            target = target.MakeAddress( compilation );
            rTarget = rTarget.MakeAddress( compilation );

            string instrKey =
                target.TypeDefinition.Name == HLBaseTypeNames.s_FloatTypeName ||
                rTarget.TypeDefinition.Name == HLBaseTypeNames.s_FloatTypeName
                    ? InstructionKey + ".F"
                    : InstructionKey;

            if ( target.IsPointer )
            {
                ExpressionTarget et = new ExpressionTarget(
                    compilation.GetTempVarDref( target.ResultAddress ),
                    true,
                    target.TypeDefinition
                );

                compilation.EmitterResult.Emit(
                    instrKey,
                    et.ResultAddress,
                    rTarget.ResultAddress,
                    outputTarget.ResultAddress
                );

                compilation.ReleaseTempVar( et.ResultAddress );
                compilation.ReleaseTempVar( rTarget.ResultAddress );
                compilation.ReleaseTempVar( target.ResultAddress );

                return outputTarget;
            }

            compilation.EmitterResult.Emit(
                instrKey,
                target.ResultAddress,
                rTarget.ResultAddress,
                outputTarget.ResultAddress
            );

            compilation.ReleaseTempVar( rTarget.ResultAddress );
            compilation.ReleaseTempVar( target.ResultAddress );

            return outputTarget;
        }

        #endregion

        #region Protected

        protected override ExpressionTarget ComputeStatic(
            HlCompilation compilation,
            ExpressionTarget left,
            ExpressionTarget right )
        {
            return new ExpressionTarget( $"{left.StaticParse() / right.StaticParse()}", false, left.TypeDefinition );
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
