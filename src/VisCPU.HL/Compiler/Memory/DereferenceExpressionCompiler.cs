using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Memory
{

    public class DereferenceExpressionCompiler : HlExpressionCompiler < HlUnaryOp >
    {
        protected override bool NeedsOutput => true;

        #region Public

        public static ExpressionTarget Emit(
            HlCompilation compilation,
            ExpressionTarget target,
            ExpressionTarget outputTarget )
        {
            compilation.EmitterResult.Emit(
                $"DREF",
                target.ResultAddress,
                outputTarget.ResultAddress
            );

            return outputTarget.Cast( target.TypeDefinition );
        }

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
            HlUnaryOp expr,
            ExpressionTarget outputTarget )
        {
            return Emit( compilation, compilation.Parse( expr.Left ), outputTarget );
        }

        #endregion
    }

}
