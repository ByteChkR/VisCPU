using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.Utility.Settings;

namespace VisCPU.HL.Compiler.Math
{

    public abstract class MathExpressionCompiler : HLExpressionCompiler < HLBinaryOp >
    {

        protected abstract string InstructionKey { get; }

        protected override bool NeedsOutput => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLBinaryOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget target = compilation.Parse( expr.Left );
            ExpressionTarget rTarget = compilation.Parse( expr.Right );

            if ( SettingsSystem.GetSettings < HLCompilerSettings >().OptimizeConstExpressions &&
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

                compilation.ProgramCode.Add(
                                            $"{InstructionKey} {et.ResultAddress} {rTarget.ResultAddress} {outputTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );

                compilation.ReleaseTempVar( et.ResultAddress );
                compilation.ReleaseTempVar( rTarget.ResultAddress );
                compilation.ReleaseTempVar( target.ResultAddress );

                return outputTarget;
            }

            compilation.ProgramCode.Add(
                                        $"{InstructionKey} {target.ResultAddress} {rTarget.ResultAddress} {outputTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                       );

            compilation.ReleaseTempVar( rTarget.ResultAddress );
            compilation.ReleaseTempVar( target.ResultAddress );

            return outputTarget;
        }

        #endregion

        #region Protected

        protected abstract ExpressionTarget ComputeStatic(
            HLCompilation compilation,
            ExpressionTarget left,
            ExpressionTarget right );

        #endregion

    }

}
