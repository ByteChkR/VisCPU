using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Logic
{

    public class EqualExpressionCompiler : HlExpressionCompiler < HlBinaryOp >
    {

        #region Public

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlBinaryOp expr )
        {


            ExpressionTarget targetVal = compilation.Parse(
                                                           expr.Left
                                                          );
            ExpressionTarget target = targetVal.
                MakeAddress(compilation);

            ExpressionTarget rTargetVal = compilation.Parse(
                                                            expr.Right,
                                                            !target.IsPointer ? target : default
                                                           );
            ExpressionTarget rTarget = rTargetVal.
                MakeAddress(compilation);

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

                compilation.ReleaseTempVar(rTarget.ResultAddress);
                compilation.ReleaseTempVar(rTargetVal.ResultAddress);

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
                                                                      compilation.TypeSystem.GetType(
                                                                           compilation.Root,
                                                                           HLBaseTypeNames.s_UintTypeName
                                                                          )
                                                                     );

                    compilation.EmitterResult.Emit(
                                                   $"CREF",
                                                   tmpTarget.ResultAddress,
                                                   target.ResultAddress
                                                  );

                    compilation.ReleaseTempVar(tmpTarget.ResultAddress);
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
                                                                      compilation.TypeSystem.GetType(
                                                                           compilation.Root,
                                                                           HLBaseTypeNames.s_UintTypeName
                                                                          )
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

            compilation.ReleaseTempVar(rTarget.ResultAddress);
            compilation.ReleaseTempVar(rTargetVal.ResultAddress);

            return target;
        }

        #endregion

    }

}
