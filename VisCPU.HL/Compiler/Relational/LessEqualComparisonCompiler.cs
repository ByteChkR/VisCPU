using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Relational
{

    public class LessEqualComparisonCompiler : HLExpressionCompiler < HLBinaryOp >
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


            string label = compilation.GetUniqueName( "bexpr_le" );
            compilation.ProgramCode.Add( $"BGT {target.ResultAddress} {rTarget.ResultAddress} {label}" );
            compilation.ProgramCode.Add( $"LOAD {outputTarget.ResultAddress} 1" );
            compilation.ProgramCode.Add( $".{label} linker:hide" );

            compilation.ReleaseTempVar(rTarget.ResultAddress);
            compilation.ReleaseTempVar(target.ResultAddress);
            return outputTarget;
        }

        #endregion

    }

}
