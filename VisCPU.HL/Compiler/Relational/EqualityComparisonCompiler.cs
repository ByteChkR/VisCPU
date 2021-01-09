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
                                                                              compilation.GetTempVar(0),
                                                                              true,
                                                                              compilation.TypeSystem.GetType( "var" )
                                                                             )
                                                        );

            if ( target.IsPointer )
            {
                ExpressionTarget tmp = new ExpressionTarget(
                                                            compilation.GetTempVarDref(target.ResultAddress),
                                                            true,
                                                            compilation.TypeSystem.GetType( "var" )
                                                           );

                compilation.ReleaseTempVar( target.ResultAddress );
                target = tmp;
            }

            if ( rTarget.IsPointer )
            {
                ExpressionTarget tmp = new ExpressionTarget(
                                                            compilation.GetTempVarDref(rTarget.ResultAddress),
                                                            true,
                                                            compilation.TypeSystem.GetType( "var" )
                                                           );

                
                compilation.ReleaseTempVar( rTarget.ResultAddress );
                rTarget = tmp;
            }
            

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
