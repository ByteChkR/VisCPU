using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Math.Atomic
{

    public class PostfixIncrementExpressionCompiler : HlExpressionCompiler < HlUnaryOp >
    {

        #region Public

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlUnaryOp expr )
        {
            ExpressionTarget target = compilation.Parse( expr.Left );

            string instrKey =
                target.TypeDefinition.Name == HLBaseTypeNames.s_FloatTypeName
                    ? "INC.F"
                    : "INC";

            ExpressionTarget copy = new ExpressionTarget(
                                                         compilation.GetTempVarCopy( target.ResultAddress ),
                                                         target.IsAddress,
                                                         target.TypeDefinition,
                                                         target.IsPointer
                                                        );

            compilation.EmitterResult.Emit( //Increment Original and pass incremented copy
                                           instrKey,
                                           target.ResultAddress
                                          );
            compilation.ReleaseTempVar(target.ResultAddress);

            return copy;
        }

        #endregion

    }

}
