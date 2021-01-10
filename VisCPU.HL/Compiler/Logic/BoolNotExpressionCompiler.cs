﻿using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Logic
{

    public class BoolNotExpressionCompiler : HLExpressionCompiler < HLUnaryOp >
    {

        protected override bool AllImplementations => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLUnaryOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget target = compilation.Parse(
                                                        expr.Left,
                                                        new ExpressionTarget(
                                                                             compilation.GetTempVar( 0 ),
                                                                             true,
                                                                             compilation.TypeSystem.GetType( "var" )
                                                                            )
                                                       );

            //BNE target rTarget if_b0_fail
            //LOAD possibleTarget 0x1; True Value
            //.if_b0_fail
            string label = compilation.GetUniqueName( "bexpr_not" );
            compilation.ProgramCode.Add( $"LOAD {outputTarget.ResultAddress} 1" );
            compilation.ProgramCode.Add( $"BEZ {target.ResultAddress} {label}" );
            compilation.ProgramCode.Add( $"LOAD {outputTarget.ResultAddress} 0" );
            compilation.ProgramCode.Add( $".{label} linker:hide" );
            compilation.ReleaseTempVar( target.ResultAddress );

            return outputTarget;
        }

        #endregion

    }

}
