using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Bitwise
{

    public class BitwiseInvertExpressionCompiler : HLExpressionCompiler<HLUnaryOp>
    {

        protected override bool AllImplementations => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLUnaryOp expr,
            ExpressionTarget outputTarget)
        {
            ExpressionTarget target = compilation.Parse(
                                                        expr.Left,
                                                        new ExpressionTarget(
                                                                             compilation.GetTempVar(),
                                                                             true,
                                                                             compilation.TypeSystem.GetType("var")
                                                                            )
                                                       );

            //BNE target rTarget if_b0_fail
            //LOAD possibleTarget 0x1; True Value
            //.if_b0_fail
            string tmp = compilation.GetTempVar();

            compilation.ProgramCode.Add($"LOAD {tmp} 0xFFFFFFFF");
            compilation.ProgramCode.Add($"XOR {target} {tmp}");

            return target.CopyIfNotNull(compilation, outputTarget, true);
        }

        #endregion

    }
}
