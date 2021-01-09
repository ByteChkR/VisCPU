using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Relational
{

    public class EqualityComparisonCompiler : HLExpressionCompiler < HLBinaryOp >
    {

        protected override bool NeedsOutput => true;

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
                                                                              compilation.GetTempVar(),
                                                                              true,
                                                                              compilation.TypeSystem.GetType( "var" )
                                                                             )
                                                        );

            if ( target.IsPointer )
            {
                ExpressionTarget tmp = new ExpressionTarget(
                                                            compilation.GetTempVar(),
                                                            true,
                                                            compilation.TypeSystem.GetType( "var" )
                                                           );

                compilation.ProgramCode.Add(
                                            $"DREF {target.ResultAddress} {tmp.ResultAddress} ; Dereference Array Pointer (Equality Comparison)"
                                           );

                compilation.ReleaseTempVar( target.ResultAddress );
                target = tmp;
            }

            if ( rTarget.IsPointer )
            {
                ExpressionTarget tmp = new ExpressionTarget(
                                                            compilation.GetTempVar(),
                                                            true,
                                                            compilation.TypeSystem.GetType( "var" )
                                                           );

                compilation.ProgramCode.Add(
                                            $"DREF {rTarget.ResultAddress} {tmp.ResultAddress} ; Dereference Array Pointer (Equality Comparison)"
                                           );

                compilation.ReleaseTempVar( rTarget.ResultAddress );
                rTarget = tmp;
            }

            //BNE target rTarget if_b0_fail
            //LOAD possibleTarget 0x1; True Value
            //.if_b0_fail

            string label = compilation.GetUniqueName( "bexpr_eq" );
            compilation.ProgramCode.Add( $"LOAD {outputTarget.ResultAddress} 0" );
            compilation.ProgramCode.Add( $"BNE {target.ResultAddress} {rTarget.ResultAddress} {label}" );
            compilation.ProgramCode.Add( $"LOAD {outputTarget.ResultAddress} 1" );
            compilation.ProgramCode.Add( $".{label} linker:hide" );
            compilation.ReleaseTempVar( rTarget.ResultAddress );
            compilation.ReleaseTempVar( target.ResultAddress );

            return outputTarget;
        }

        #endregion

    }

}
