﻿using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Compiler.Special
{

    public class IfBlockCompiler : HLExpressionCompiler < HLIfOp >
    {

        #region Public

        public override ExpressionTarget ParseExpression( HLCompilation compilation, HLIfOp expr )
        {
            string endLabel = compilation.GetUniqueName( "if_end" );
            string elseLabel = compilation.GetUniqueName( "if_else" );
            string blockLabels = compilation.GetUniqueName( "if_b{0}" );

            for ( int i = 0; i < expr.ConditionMap.Count; i++ )
            {
                string thisLabel = string.Format( blockLabels, i );

                if ( i != 0 )
                {
                    compilation.ProgramCode.Add( $".{thisLabel} linker:hide" );
                }
                else
                {
                    compilation.ProgramCode.Add( "; Start IF" );
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

                compilation.ProgramCode.Add( $"BEZ {exprTarget.ResultAddress} {nextLabel}" );

                foreach ( HLExpression hlExpression in expr.ConditionMap[i].Item2 )
                {
                    compilation.Parse( hlExpression );
                }

                compilation.ProgramCode.Add( $"JMP {endLabel}" );
                compilation.ReleaseTempVar( exprTarget.ResultAddress );
            }

            if ( expr.ElseBranch != null )
            {
                compilation.ProgramCode.Add( $".{elseLabel} linker:hide" );

                foreach ( HLExpression hlExpression in expr.ElseBranch )
                {
                    compilation.Parse( hlExpression );
                }
            }

            compilation.ProgramCode.Add( $".{endLabel} linker:hide" );

            return new ExpressionTarget();
        }

        #endregion

    }

}
