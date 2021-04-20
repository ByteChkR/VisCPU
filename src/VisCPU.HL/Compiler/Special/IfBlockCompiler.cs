using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility.IO.Settings;

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

            bool staticComputation = false;

            compilation.EmitterResult.Store( "; Start IF" );

            for ( int i = 0; i < expr.ConditionMap.Count; i++ )
            {
                string thisLabel = string.Format( blockLabels, i );
                HlCompilation subIf = new HlCompilation( compilation, HlCompilation.GetUniqueName( thisLabel ) );

                if ( SettingsManager.GetSettings < HlCompilerSettings >().OptimizeIfConditionExpressions &&
                     expr.ConditionMap[i].Item1.IsStatic() )
                {
                    ExpressionTarget t = subIf.Parse(
                                                     expr.ConditionMap[i].Item1
                                                    );

                    if ( t.StaticParse() != 0 )
                    {
                        staticComputation = true;

                        foreach ( HlExpression hlExpression in expr.ConditionMap[i].Item2 )
                        {
                            subIf.Parse( hlExpression );
                        }

                        break;
                    }

                    if ( i != 0 )
                    {
                        subIf.EmitterResult.Store( $".{thisLabel} linker:hide" );
                    }

                    continue;
                }

                if ( i != 0 )
                {
                    subIf.EmitterResult.Store( $".{thisLabel} linker:hide" );
                }

                ExpressionTarget exprTarget = subIf.Parse(
                                                          expr.ConditionMap[i].Item1
                                                         ).
                                                    MakeAddress( subIf );

                string nextLabel;

                if ( i < expr.ConditionMap.Count - 1 )
                {
                    nextLabel = string.Format( blockLabels, i + 1 );
                }
                else
                {
                    nextLabel = expr.ElseBranch != null ? elseLabel : endLabel;
                }

                subIf.EmitterResult.Emit( $"BEZ", exprTarget.ResultAddress, nextLabel );

                foreach ( HlExpression hlExpression in expr.ConditionMap[i].Item2 )
                {
                    subIf.Parse( hlExpression );
                }

                subIf.EmitterResult.Emit( $"JMP", endLabel );
                subIf.ReleaseTempVar( exprTarget.ResultAddress );

                compilation.EmitterResult.Store( subIf.EmitVariables( false ) );
                compilation.EmitterResult.Store( subIf.EmitterResult.Get() );
            }

            if ( !staticComputation && expr.ElseBranch != null )
            {
                HlCompilation subIf = new HlCompilation( compilation, elseLabel );
                subIf.EmitterResult.Store( $".{elseLabel} linker:hide" );

                foreach ( HlExpression hlExpression in expr.ElseBranch )
                {
                    subIf.Parse( hlExpression );
                }

                compilation.EmitterResult.Store( subIf.EmitVariables( false ) );
                compilation.EmitterResult.Store( subIf.EmitterResult.Get() );
            }

            compilation.EmitterResult.Store( $".{endLabel} linker:hide" );

            return new ExpressionTarget();
        }

        #endregion

    }

}
