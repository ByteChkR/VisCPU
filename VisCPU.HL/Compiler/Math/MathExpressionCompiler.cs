using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Math
{

    public abstract class MathExpressionCompiler : HLExpressionCompiler < HLBinaryOp >
    {

        protected abstract string InstructionKey { get; }

        protected override bool NeedsOutput => true;

        #region Public

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLBinaryOp expr,
            ExpressionTarget outputTarget )
        {
            ExpressionTarget target = compilation.Parse( expr.Left, outputTarget );

            ExpressionTarget rTarget = compilation.Parse(
                                                         expr.Right,
                                                         new ExpressionTarget(
                                                                              compilation.GetTempVar( 0 ),
                                                                              true,
                                                                              compilation.TypeSystem.GetType( "var" )
                                                                             )
                                                        );

            if ( target.ResultAddress == outputTarget.ResultAddress )
            {
                compilation.ProgramCode.Add(
                                            $"{InstructionKey} {target.ResultAddress} {rTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );

                compilation.ReleaseTempVar( rTarget.ResultAddress );

                return target;
            }

            if ( target.IsPointer )
            {
                ExpressionTarget et = new ExpressionTarget(
                                                           compilation.GetTempVarDref( target.ResultAddress ),
                                                           true,
                                                           compilation.TypeSystem.GetType( "var" )
                                                          );

                compilation.ProgramCode.Add(
                                            $"{InstructionKey} {et.ResultAddress} {rTarget.ResultAddress} {outputTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );

                compilation.ReleaseTempVar( et.ResultAddress );
                compilation.ReleaseTempVar( rTarget.ResultAddress );
                compilation.ReleaseTempVar( target.ResultAddress );

                return outputTarget;
            }

            compilation.ProgramCode.Add(
                                        $"{InstructionKey} {target.ResultAddress} {rTarget.ResultAddress} {outputTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                       );

            compilation.ReleaseTempVar( rTarget.ResultAddress );
            compilation.ReleaseTempVar( target.ResultAddress );

            return outputTarget;
        }

        #endregion

    }

}
