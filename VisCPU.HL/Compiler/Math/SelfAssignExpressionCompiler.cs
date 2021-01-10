using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Math
{

    public abstract class SelfAssignExpressionCompiler : HLExpressionCompiler < HLBinaryOp >
    {

        protected abstract string InstructionKey { get; }

        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLBinaryOp expr )
        {
            ExpressionTarget target = compilation.Parse( expr.Left );

            ExpressionTarget rTarget = compilation.Parse(
                                                         expr.Right,
                                                         new ExpressionTarget(
                                                                              compilation.GetTempVar( 0 ),
                                                                              true,
                                                                              compilation.TypeSystem.GetType( "var" )
                                                                             )
                                                        );

            compilation.ProgramCode.Add(
                                        $"{InstructionKey} {target.ResultAddress} {rTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                       );

            compilation.ReleaseTempVar( rTarget.ResultAddress );

            return target;
        }

        #endregion

    }

}
