using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Math.Full
{

    public class MulExpressionCompiler : MathExpressionCompiler
    {

        protected override string InstructionKey => "MUL";

        #region Public

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
            HlBinaryOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget targetVal = compilation.Parse( expr.Left );
            ExpressionTarget rTargetVal = compilation.Parse( expr.Right );

            if ( SettingsManager.GetSettings < HlCompilerSettings >().OptimizeConstExpressions &&
                 !targetVal.IsAddress &&
                 !rTargetVal.IsAddress )
            {
                return ComputeStatic( compilation, targetVal, rTargetVal);
            }

            if ( SettingsManager.GetSettings < HlCompilerSettings >().OptimizeConstExpressions )
            {
                uint amount = 0;
                ExpressionTarget baseExpr = default;

                if ( IsReducable(targetVal) )
                {
                    baseExpr = rTargetVal.MakeAddress( compilation );
                    amount = GetPowLevel(targetVal.StaticParse() );
                }
                else if ( IsReducable(rTargetVal) )
                {
                    baseExpr = targetVal.MakeAddress( compilation );
                    amount = GetPowLevel(rTargetVal.StaticParse() );
                }

                if ( amount != 0 )
                {
                    string tmp = compilation.GetTempVarLoad( amount.ToString() );

                    compilation.EmitterResult.Emit(
                                                   "SHL",
                                                   baseExpr.ResultAddress,
                                                   tmp,
                                                   outputTarget.ResultAddress
                                                  );

                    compilation.ReleaseTempVar(tmp);
                    compilation.ReleaseTempVar(baseExpr.ResultAddress);
                    compilation.ReleaseTempVar(targetVal.ResultAddress);
                    compilation.ReleaseTempVar(rTargetVal.ResultAddress);

                    return outputTarget;
                }
            }

           ExpressionTarget target = targetVal.MakeAddress( compilation );
           ExpressionTarget rTarget = rTargetVal.MakeAddress( compilation );

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
                compilation.ReleaseTempVar(targetVal.ResultAddress);
                compilation.ReleaseTempVar(rTargetVal.ResultAddress);

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
            compilation.ReleaseTempVar(targetVal.ResultAddress);
            compilation.ReleaseTempVar(rTargetVal.ResultAddress);

            return outputTarget;
        }

        #endregion

        #region Protected

        protected override ExpressionTarget ComputeStatic(
            HlCompilation compilation,
            ExpressionTarget left,
            ExpressionTarget right )
        {
            return new ExpressionTarget( $"{left.StaticParse() * right.StaticParse()}", false, left.TypeDefinition );
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
