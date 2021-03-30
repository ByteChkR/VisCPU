using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Relational
{

    public abstract class RelationalExpressionCompiler : HlExpressionCompiler < HlBinaryOp >
    {
        protected override bool NeedsOutput => true;

        protected abstract string InstructionKey { get; }

        #region Public

        public abstract uint StaticEvaluate( ExpressionTarget a, ExpressionTarget b );

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
            HlBinaryOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget target = compilation.Parse(
                expr.Left
            );

            ExpressionTarget rTarget = compilation.Parse(
                expr.Right
            );

            if ( SettingsManager.GetSettings < HlCompilerSettings >().OptimizeConstExpressions &&
                 !target.IsAddress &&
                 !rTarget.IsAddress )
            {
                return new ExpressionTarget(
                    StaticEvaluate( target, rTarget ).ToString(),
                    false,
                    compilation.TypeSystem.GetType( compilation.Root, HLBaseTypeNames.s_UintTypeName )
                );
            }

            target = target.MakeAddress( compilation );
            rTarget = rTarget.MakeAddress( compilation );

            if ( target.IsPointer )
            {
                ExpressionTarget tmp = new ExpressionTarget(
                    compilation.GetTempVarDref( target.ResultAddress ),
                    true,
                    compilation.TypeSystem.GetType(
                        compilation.Root,
                        HLBaseTypeNames.s_UintTypeName
                    )
                );

                compilation.ReleaseTempVar( target.ResultAddress );
                target = tmp;
            }

            if ( rTarget.IsPointer )
            {
                ExpressionTarget tmp = new ExpressionTarget(
                    compilation.GetTempVarDref( rTarget.ResultAddress ),
                    true,
                    compilation.TypeSystem.GetType(
                        compilation.Root,
                        HLBaseTypeNames.s_UintTypeName
                    )
                );

                compilation.ReleaseTempVar( rTarget.ResultAddress );
                rTarget = tmp;
            }

            //BNE target rTarget if_b0_fail
            //LOAD possibleTarget 0x1; True Value
            //.if_b0_fail
            string label = HlCompilation.GetUniqueName( "rel_expr_comp" );
            compilation.EmitterResult.Emit( $"LOAD", outputTarget.ResultAddress, "1" );

            compilation.EmitterResult.Store(
                $"{InstructionKey} {target.ResultAddress} {rTarget.ResultAddress} {label}"
            );

            compilation.EmitterResult.Emit( $"LOAD", outputTarget.ResultAddress, "0" );
            compilation.EmitterResult.Store( $".{label} linker:hide" );
            compilation.ReleaseTempVar( rTarget.ResultAddress );
            compilation.ReleaseTempVar( target.ResultAddress );

            return outputTarget;
        }

        #endregion
    }

}
