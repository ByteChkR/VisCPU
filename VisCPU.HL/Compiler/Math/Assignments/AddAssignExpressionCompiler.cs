using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Math.Assignments
{

    public abstract class SelfAssignExpressionCompiler : HLExpressionCompiler<HLBinaryOp>
    {

        protected abstract string InstructionKey { get; }
        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLBinaryOp expr)
        {
            ExpressionTarget target = compilation.Parse(expr.Left);

            ExpressionTarget rTarget = compilation.Parse(
                                                         expr.Right,
                                                         new ExpressionTarget(
                                                                              compilation.GetTempVar(),
                                                                              true,
                                                                              compilation.TypeSystem.GetType("var")
                                                                             )
                                                        );


            compilation.ProgramCode.Add(
                                        $"{InstructionKey} {target.ResultAddress} {rTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                       );

            compilation.ReleaseTempVar(rTarget.ResultAddress);

            return target;
        }

        #endregion

    }
    public class SubAssignExpressionCompiler : SelfAssignExpressionCompiler
    {
        protected override string InstructionKey => "SUB";
    }
    public class AddAssignExpressionCompiler : SelfAssignExpressionCompiler
    {
        protected override string InstructionKey => "ADD";
    }
    public class DivAssignExpressionCompiler : SelfAssignExpressionCompiler
    {
        protected override string InstructionKey => "DIV";
    }
    public class ModAssignExpressionCompiler : SelfAssignExpressionCompiler
    {
        protected override string InstructionKey => "MOD";
    }
    public class MulAssignExpressionCompiler : SelfAssignExpressionCompiler
    {
        protected override string InstructionKey => "MUL";
    }
    public class ShiftLeftAssignExpressionCompiler : SelfAssignExpressionCompiler
    {
        protected override string InstructionKey => "SHL";
    }
    public class ShiftRightAssignExpressionCompiler : SelfAssignExpressionCompiler
    {
        protected override string InstructionKey => "SHR";
    }
    public class AndAssignExpressionCompiler : SelfAssignExpressionCompiler
    {
        protected override string InstructionKey => "AND";
    }
    public class OrAssignExpressionCompiler : SelfAssignExpressionCompiler
    {
        protected override string InstructionKey => "OR";
    }
    public class XOrAssignExpressionCompiler : SelfAssignExpressionCompiler
    {
        protected override string InstructionKey => "XOR";
    }


}
