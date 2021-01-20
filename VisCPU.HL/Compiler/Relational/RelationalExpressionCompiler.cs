using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Relational
{

    public abstract class RelationalExpressionCompiler : HLExpressionCompiler < HLBinaryOp >
    {
        protected override bool NeedsOutput => true;

        protected abstract string InstructionKey { get; }

        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLBinaryOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget target = compilation.Parse(
                                                      expr.Left
                                                  ).
                                                  MakeAddress( compilation );

            ExpressionTarget rTarget = compilation.Parse(
                expr.Right,
                new ExpressionTarget(
                    compilation.GetTempVar( 0 ),
                    true,
                    compilation.TypeSystem.GetType( "var" )
                )
            );

            if ( target.IsPointer )
            {
                ExpressionTarget tmp = new ExpressionTarget(
                    compilation.GetTempVarDref( target.ResultAddress ),
                    true,
                    compilation.TypeSystem.GetType( "var" )
                );

                compilation.ReleaseTempVar( target.ResultAddress );
                target = tmp;
            }

            if ( rTarget.IsPointer )
            {
                ExpressionTarget tmp = new ExpressionTarget(
                    compilation.GetTempVarDref( rTarget.ResultAddress ),
                    true,
                    compilation.TypeSystem.GetType( "var" )
                );

                compilation.ReleaseTempVar( rTarget.ResultAddress );
                rTarget = tmp;
            }

            //BNE target rTarget if_b0_fail
            //LOAD possibleTarget 0x1; True Value
            //.if_b0_fail
            string label = HLCompilation.GetUniqueName( "rel_expr_comp" );
            compilation.EmitterResult.Emit( $"LOAD", outputTarget.ResultAddress, "1" );

            compilation.EmitterResult.Store(
                $"{InstructionKey} {target.ResultAddress} {rTarget.ResultAddress} {label}"
            );

            compilation.EmitterResult.Emit( $"LOAD", outputTarget.ResultAddress, "0" );
            compilation.EmitterResult.Store( $".{label} linker:hide" );
            compilation.ReleaseTempVar( rTarget.ResultAddress );
            compilation.ReleaseTempVar( target.ResultAddress );

            return outputTarget;
        }

        #endregion
    }

}
