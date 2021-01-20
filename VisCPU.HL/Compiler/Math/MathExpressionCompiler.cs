using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.Utility.Settings;

namespace VisCPU.HL.Compiler.Math
{

    public abstract class MathExpressionCompiler : HlExpressionCompiler < HlBinaryOp >
    {
        protected abstract string InstructionKey { get; }

        protected override bool NeedsOutput => true;

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

            target = target.MakeAddress( compilation );
            rTarget = rTarget.MakeAddress( compilation );

            if ( target.IsPointer )
            {
                ExpressionTarget et = new ExpressionTarget(
                    compilation.GetTempVarDref( target.ResultAddress ),
                    true,
                    compilation.TypeSystem.GetType( "var" )
                );

                compilation.EmitterResult.Emit(
                    InstructionKey,
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
                InstructionKey,
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

        protected abstract ExpressionTarget ComputeStatic(
            HlCompilation compilation,
            ExpressionTarget left,
            ExpressionTarget right );

        #endregion
    }

}
