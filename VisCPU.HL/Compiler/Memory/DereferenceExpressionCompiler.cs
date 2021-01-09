using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Memory
{

    public class DereferenceExpressionCompiler : HLExpressionCompiler<HLUnaryOp>
    {

        protected override bool NeedsOutput => true;

        public static ExpressionTarget Emit(HLCompilation compilation, ExpressionTarget target, ExpressionTarget outputTarget)
        {
            compilation.ProgramCode.Add(
                                        $"DREF {target.ResultAddress} {outputTarget.ResultAddress}; Dereference"
                                       );

            return outputTarget;
        }

        public override ExpressionTarget ParseExpression(HLCompilation compilation, HLUnaryOp expr, ExpressionTarget outputTarget)
        {
            return Emit(compilation, compilation.Parse(expr.Left), outputTarget);
        }

    }
    public class ReferenceExpressionCompiler : HLExpressionCompiler<HLUnaryOp>
    {

        protected override bool NeedsOutput => true;

        public static ExpressionTarget Emit(HLCompilation compilation, ExpressionTarget target, ExpressionTarget outputTarget)
        {
            compilation.ProgramCode.Add(
                                        $"LOAD {outputTarget.ResultAddress} {target.ResultAddress}; Reference"
                                       );

            return outputTarget;
        }

        public override ExpressionTarget ParseExpression(HLCompilation compilation, HLUnaryOp expr, ExpressionTarget outputTarget)
        {
            return Emit(compilation, compilation.Parse(expr.Left), outputTarget);
        }

    }

}
