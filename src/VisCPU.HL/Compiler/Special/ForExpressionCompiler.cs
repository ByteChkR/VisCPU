using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler.Special
{

    public class ForExpressionCompiler : HlExpressionCompiler < HlForOp >
    {
        #region Public

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlForOp expr )
        {
            string startLabel = HlCompilation.GetUniqueName( "for_start" );
            string condLabel = HlCompilation.GetUniqueName( "for_eval_condition" );
            string endLabel = HlCompilation.GetUniqueName( "for_end" );

            compilation.EmitterResult.Store(
                $".{startLabel} linker:hide" ); //Unused, makes clear where the for compiler started emitting code.

            ExpressionTarget
                target = compilation.Parse( expr.VDecl ).MakeAddress( compilation ); //Declare Variable(left)

            compilation.EmitterResult.Store( $".{condLabel} linker:hide" ); //Label to jumpto

            ExpressionTarget cond = compilation.Parse( expr.Condition ).MakeAddress( compilation ); //Check Condition

            compilation.EmitterResult.Emit(
                $"BEZ",
                cond.ResultAddress,
                endLabel ); //Check if Expression "Equal to Zero" => jump to end if it is

            foreach ( HlExpression hlExpression in expr.ExprBody )
            {
                compilation.ReleaseTempVar(
                    compilation.Parse( hlExpression ).
                                ResultAddress ); //Parse block and clean up any temp variables that were emitted.
            }

            compilation.ReleaseTempVar( compilation.Parse( expr.VInc ).ResultAddress ); //Compute Increment Expression

            compilation.EmitterResult.Emit( $"JMP", condLabel ); //Jump back up if we executed the body.

            compilation.EmitterResult.Store(
                $".{endLabel} linker:hide" ); //End label that we jump to if we exit the loop

            compilation.ReleaseTempVar( target.ResultAddress );
            compilation.ReleaseTempVar( cond.ResultAddress );

            return new ExpressionTarget();
        }

        #endregion
    }

}
