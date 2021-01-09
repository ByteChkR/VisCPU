using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Math
{

    public class IncrementExpressionCompiler : HLExpressionCompiler<HLUnaryOp>
    {

        public override ExpressionTarget ParseExpression(HLCompilation compilation, HLUnaryOp expr)
        {
            ExpressionTarget target = compilation.Parse(expr.Left);


            compilation.ProgramCode.Add(
                                        $"INC {target.ResultAddress}; Increment: {expr.Left}"
                                       );

            return target;
        }

    }
    public class DecrementExpressionCompiler : HLExpressionCompiler<HLUnaryOp>
    {

        public override ExpressionTarget ParseExpression(HLCompilation compilation, HLUnaryOp expr)
        {
            ExpressionTarget target = compilation.Parse(expr.Left);


            compilation.ProgramCode.Add(
                                        $"DEC {target.ResultAddress}; Increment: {expr.Left}"
                                       );

            return target;
        }

    }

}
