using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler.Special
{

    public class WhileExpressionCompiler : HLExpressionCompiler < HLWhileOp >
    {
        #region Public

        public override ExpressionTarget ParseExpression( HLCompilation compilation, HLWhileOp expr )
        {
            string startLabel = HLCompilation.GetUniqueName( "while_start" );
            string endLabel = HLCompilation.GetUniqueName( "while_end" );

            compilation.EmitterResult.Store( $".{startLabel} linker:hide" );

            ExpressionTarget target = compilation.Parse( expr.Condition ).MakeAddress( compilation );

            compilation.EmitterResult.Emit( $"BEZ", target.ResultAddress, endLabel );

            foreach ( HLExpression hlExpression in expr.Block )
            {
                compilation.Parse( hlExpression );
            }

            compilation.EmitterResult.Emit( $"JMP", startLabel );
            compilation.EmitterResult.Store( $".{endLabel} linker:hide" );

            compilation.ReleaseTempVar( target.ResultAddress );

            //.while_start
            //LOAD tmp_condition_var 0x00
            //<PARSE CONDITION EXPR HERE>
            //BEZ tmp_condition_var while_end
            //<PARSE BLOCK HERE>
            //JMP while_start
            //.while_end
            return new ExpressionTarget();
        }

        #endregion
    }

}
