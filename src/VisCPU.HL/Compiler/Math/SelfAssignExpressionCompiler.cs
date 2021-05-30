using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.Utility.SharedBase;

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

            ExpressionTarget rTargetVal = compilation.Parse(
                                                            expr.Right
                                                           );
                ExpressionTarget rTarget = rTargetVal.MakeAddress(compilation);

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

            compilation.ReleaseTempVar(rTarget.ResultAddress);
            compilation.ReleaseTempVar(rTargetVal.ResultAddress);

            return target;
        }

        #endregion

    }

}
