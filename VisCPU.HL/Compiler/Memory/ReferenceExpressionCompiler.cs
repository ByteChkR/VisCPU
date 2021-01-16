using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Memory
{

    public class ReferenceExpressionCompiler : HLExpressionCompiler < HLUnaryOp >
    {

        protected override bool NeedsOutput => true;

        #region Public

        public static ExpressionTarget Emit(
            HLCompilation compilation,
            ExpressionTarget target,
            ExpressionTarget outputTarget )
        {
            compilation.EmitterResult.Emit(
                                           $"LOAD",
                                           outputTarget.ResultAddress,
                                           target.ResultAddress
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
