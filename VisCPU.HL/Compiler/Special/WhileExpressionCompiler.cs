using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler.Special
{

    public class WhileExpressionCompiler : HLExpressionCompiler < HLWhileOp >
    {

        #region Public

        public override ExpressionTarget ParseExpression( HLCompilation compilation, HLWhileOp expr )
        {
            string startLabel = compilation.GetUniqueName( "while_start" );
            string endLabel = compilation.GetUniqueName( "while_end" );

            compilation.ProgramCode.Add( $".{startLabel} linker:hide" );
            string tname = compilation.GetTempVar();
            ExpressionTarget target = new ExpressionTarget( tname, true, compilation.TypeSystem.GetType( "var" ) );
            compilation.ProgramCode.Add( $"LOAD {target.ResultAddress} 0x00" );
            compilation.Parse( expr.Condition, target );

            compilation.ProgramCode.Add( $"BEZ {target.ResultAddress} {endLabel}" );

            foreach ( HLExpression hlExpression in expr.Block )
            {
                compilation.Parse( hlExpression );
            }

            compilation.ProgramCode.Add( $"JMP {startLabel}" );
            compilation.ProgramCode.Add( $".{endLabel} linker:hide" );

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
