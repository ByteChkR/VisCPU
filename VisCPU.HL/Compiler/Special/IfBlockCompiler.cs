using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler.Special
{

    public class IfBlockCompiler : HlExpressionCompiler < HlIfOp >
    {
        #region Public

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlIfOp expr )
        {
            string endLabel = HlCompilation.GetUniqueName( "if_end" );
            string elseLabel = HlCompilation.GetUniqueName( "if_else" );
            string blockLabels = HlCompilation.GetUniqueName( "if_b{0}" );

            for ( int i = 0; i < expr.ConditionMap.Count; i++ )
            {
                string thisLabel = string.Format( blockLabels, i );

                if ( i != 0 )
                {
                    compilation.EmitterResult.Store( $".{thisLabel} linker:hide" );
                }
                else
                {
                    compilation.EmitterResult.Store( "; Start IF" );
                }

                ExpressionTarget exprTarget = compilation.Parse(
                                                              expr.ConditionMap[i].Item1
                                                          ).
                                                          MakeAddress( compilation );

                string nextLabel;

                if ( i < expr.ConditionMap.Count - 1 )
                {
                    nextLabel = string.Format( blockLabels, i + 1 );
                }
                else
                {
                    nextLabel = expr.ElseBranch != null ? elseLabel : endLabel;
                }

                compilation.EmitterResult.Emit( $"BEZ", exprTarget.ResultAddress, nextLabel );

                foreach ( HlExpression hlExpression in expr.ConditionMap[i].Item2 )
                {
                    compilation.Parse( hlExpression );
                }

                compilation.EmitterResult.Emit( $"JMP", endLabel );
                compilation.ReleaseTempVar( exprTarget.ResultAddress );
            }

            if ( expr.ElseBranch != null )
            {
                compilation.EmitterResult.Store( $".{elseLabel} linker:hide" );

                foreach ( HlExpression hlExpression in expr.ElseBranch )
                {
                    compilation.Parse( hlExpression );
                }
            }

            compilation.EmitterResult.Store( $".{endLabel} linker:hide" );

            return new ExpressionTarget();
        }

        #endregion
    }

}
