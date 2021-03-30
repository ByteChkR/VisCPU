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

            HlCompilation subFor = new HlCompilation( compilation, HlCompilation.GetUniqueName( "for" ) );

            subFor.EmitterResult.Store(
                $".{startLabel} linker:hide"
            ); //Unused, makes clear where the for compiler started emitting code.

            ExpressionTarget
                target = subFor.Parse( expr.VDecl ).MakeAddress( subFor ); //Declare Variable(left)

            subFor.EmitterResult.Store( $".{condLabel} linker:hide" ); //Label to jumpto

            ExpressionTarget cond = subFor.Parse( expr.Condition ).MakeAddress( subFor ); //Check Condition

            subFor.EmitterResult.Emit(
                $"BEZ",
                cond.ResultAddress,
                endLabel
            ); //Check if Expression "Equal to Zero" => jump to end if it is

            foreach ( HlExpression hlExpression in expr.ExprBody )
            {
                subFor.ReleaseTempVar(
                    subFor.Parse( hlExpression ).
                           ResultAddress
                ); //Parse block and clean up any temp variables that were emitted.
            }

            subFor.ReleaseTempVar( subFor.Parse( expr.VInc ).ResultAddress ); //Compute Increment Expression

            subFor.EmitterResult.Emit( $"JMP", condLabel ); //Jump back up if we executed the body.

            subFor.EmitterResult.Store(
                $".{endLabel} linker:hide"
            ); //End label that we jump to if we exit the loop

            subFor.ReleaseTempVar( target.ResultAddress );
            subFor.ReleaseTempVar( cond.ResultAddress );

            compilation.EmitterResult.Store( subFor.EmitVariables( false ) );
            compilation.EmitterResult.Store( subFor.EmitterResult.Get() );

            return new ExpressionTarget();
        }

        #endregion
    }

}
