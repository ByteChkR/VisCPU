using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility.Settings;

namespace VisCPU.HL.Compiler.Special
{

    public class WhileExpressionCompiler : HlExpressionCompiler < HlWhileOp >
    {
        #region Public

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlWhileOp expr )
        {
            string startLabel = HlCompilation.GetUniqueName( "while_start" );
            string endLabel = HlCompilation.GetUniqueName( "while_end" );

            compilation.EmitterResult.Store( $".{startLabel} linker:hide" );
            ExpressionTarget target = compilation.Parse( expr.Condition );

            if ( SettingsManager.GetSettings < HlCompilerSettings >().OptimizeWhileConditionExpressions &&
                 !target.IsAddress )
            {
                if ( target.StaticParse() != 0 )

                    //If True we parse body without check and directly jump to condition
                    //If False we omit the whole loop entirely
                {
                    ParseBody( compilation, expr, startLabel, endLabel );
                }

                return new ExpressionTarget();
            }

            target = target.MakeAddress( compilation ); //Make sure we have an address and not a static value

            compilation.EmitterResult.Emit( $"BEZ", target.ResultAddress, endLabel );

            ParseBody( compilation, expr, startLabel, endLabel );

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

        #region Private

        private void ParseBody( HlCompilation compilation, HlWhileOp expr, string start, string end )
        {
            foreach ( HlExpression hlExpression in expr.Block )
            {
                compilation.Parse( hlExpression );
            }

            compilation.EmitterResult.Emit( $"JMP", start );
            compilation.EmitterResult.Store( $".{end} linker:hide" );
        }

        #endregion
    }

}
