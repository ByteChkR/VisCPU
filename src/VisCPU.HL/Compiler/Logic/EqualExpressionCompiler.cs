using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Logic
{

    public class EqualExpressionCompiler : HlExpressionCompiler < HlBinaryOp >
    {
        #region Public

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlBinaryOp expr )
        {
            ExpressionTarget target = compilation.Parse( expr.Left ).MakeAddress( compilation );

            ExpressionTarget rTarget = compilation.Parse(
                                                       expr.Right,
                                                       !target.IsPointer ? target : default
                                                   ).
                                                   MakeAddress( compilation );

            if ( rTarget.ResultAddress == target.ResultAddress )
            {
                return target;
            }

            if ( !rTarget.IsAddress && !target.IsPointer )
            {
                compilation.EmitterResult.Emit(
                    $"LOAD",
                    target.ResultAddress,
                    rTarget.ResultAddress
                );

                return target;
            }

            if ( target.IsPointer )
            {
                if ( rTarget.IsPointer )
                {
                    compilation.EmitterResult.Emit(
                        $"CREF",
                        rTarget.ResultAddress,
                        target.ResultAddress
                    );
                }
                else
                {
                    ExpressionTarget tmpTarget = new ExpressionTarget(
                        compilation.GetTempVarLoad(
                            rTarget.ResultAddress
                        ),
                        true,
                        compilation.TypeSystem.GetType( HLBaseTypeNames.s_UintTypeName )
                    );

                    compilation.EmitterResult.Emit(
                        $"CREF",
                        tmpTarget.ResultAddress,
                        target.ResultAddress
                    );

                    compilation.ReleaseTempVar( tmpTarget.ResultAddress );
                }
            }
            else
            {
                if ( rTarget.IsPointer )
                {
                    ExpressionTarget tmpTarget = new ExpressionTarget(
                        compilation.GetTempVarLoad(
                            target.ResultAddress
                        ),
                        true,
                        compilation.TypeSystem.GetType( HLBaseTypeNames.s_UintTypeName )
                    );

                    compilation.EmitterResult.Emit(
                        $"CREF",
                        rTarget.ResultAddress,
                        tmpTarget.ResultAddress
                    );

                    compilation.ReleaseTempVar( tmpTarget.ResultAddress );
                }
                else if ( rTarget.ResultAddress != target.ResultAddress )
                {
                    compilation.EmitterResult.Emit(
                        $"COPY",
                        rTarget.ResultAddress,
                        target.ResultAddress
                    );
                }
            }

            compilation.ReleaseTempVar( rTarget.ResultAddress );

            return target;
        }

        #endregion
    }

}
