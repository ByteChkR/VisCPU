using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Math.Atomic
{

    public class PostfixDecrementExpressionCompiler : HlExpressionCompiler < HlUnaryOp >
    {

        #region Public

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlUnaryOp expr )
        {
            ExpressionTarget target = compilation.Parse( expr.Left );

            string instrKey =
                target.TypeDefinition.Name == HLBaseTypeNames.s_FloatTypeName
                    ? "DEC.F"
                    : "DEC";

            ExpressionTarget copy = new ExpressionTarget(
                                                         compilation.GetTempVarCopy( target.ResultAddress ),
                                                         target.IsAddress,
                                                         target.TypeDefinition,
                                                         target.IsPointer
                                                        );

            compilation.EmitterResult.Emit( //Decrement Original and pass decremented copy
                                           instrKey,
                                           target.ResultAddress
                                          );

            compilation.ReleaseTempVar( target.ResultAddress );
            return copy;
        }

        #endregion

    }

}
