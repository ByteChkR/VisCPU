using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Memory
{

    public class DereferenceExpressionCompiler : HLExpressionCompiler < HLUnaryOp >
    {

        protected override bool NeedsOutput => true;

        #region Public

        public static ExpressionTarget Emit(
            HLCompilation compilation,
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
            HLCompilation compilation,
            HLUnaryOp expr,
            ExpressionTarget outputTarget )
        {
            return Emit( compilation, compilation.Parse( expr.Left ), outputTarget );
        }

        #endregion

    }

}
