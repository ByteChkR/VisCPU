using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Math
{

    public class AddExpressionCompiler : HLExpressionCompiler < HLBinaryOp >
    {

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
                                                                              compilation.GetTempVar(),
                                                                              true,
                                                                              compilation.TypeSystem.GetType( "var" )
                                                                             )
                                                        );

            if ( target.ResultAddress == outputTarget.ResultAddress )
            {
                compilation.ProgramCode.Add(
                                            $"ADD {target.ResultAddress} {rTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );

                compilation.ReleaseTempVar( rTarget.ResultAddress );

                return target;
            }

            if ( target.IsPointer )
            {
                ExpressionTarget et = new ExpressionTarget(
                                                           compilation.GetTempVar(),
                                                           true,
                                                           compilation.TypeSystem.GetType( "var" )
                                                          );

                compilation.ProgramCode.Add(
                                            $"DREF {target.ResultAddress} {et.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );

                compilation.ProgramCode.Add(
                                            $"ADD {et.ResultAddress} {rTarget.ResultAddress} {outputTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                           );

                compilation.ReleaseTempVar(et.ResultAddress);
                compilation.ReleaseTempVar(rTarget.ResultAddress);
                compilation.ReleaseTempVar( target.ResultAddress );

                return outputTarget;
            }

            compilation.ProgramCode.Add(
                                        $"ADD {target.ResultAddress} {rTarget.ResultAddress} {outputTarget.ResultAddress}; Left: {expr.Left} ; Right: {expr.Right}"
                                       );

            compilation.ReleaseTempVar(rTarget.ResultAddress);
            compilation.ReleaseTempVar(target.ResultAddress);

            return outputTarget;
        }

        #endregion

    }

}
