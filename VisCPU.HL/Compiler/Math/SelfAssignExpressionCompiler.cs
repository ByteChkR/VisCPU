using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Math
{

    public abstract class SelfAssignExpressionCompiler : HlExpressionCompiler < HlBinaryOp >
    {
        protected abstract string InstructionKey { get; }

        #region Public

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
            HlBinaryOp expr )
        {
            ExpressionTarget target = compilation.Parse( expr.Left );

            ExpressionTarget rTarget = compilation.Parse(
                                                       expr.Right
                                                   ).
                                                   MakeAddress( compilation );

            compilation.EmitterResult.Emit(
                InstructionKey,
                target.ResultAddress,
                rTarget.ResultAddress
            );

            compilation.ReleaseTempVar( rTarget.ResultAddress );

            return target;
        }

        #endregion
    }

}
