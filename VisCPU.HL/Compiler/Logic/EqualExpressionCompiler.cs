using System.Collections.Generic;

using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler.Logic
{

    public class EqualExpressionCompiler : HLExpressionCompiler < HLBinaryOp >
    {

        #region Public

        public override ExpressionTarget ParseExpression( HLCompilation compilation, HLBinaryOp expr )
        {
            ExpressionTarget target = compilation.Parse( expr.Left ).MakeAddress(compilation);

            if ( compilation.ConstValTypes.ContainsKey( target.ResultAddress ) &&
                 compilation.ConstValTypes[target.ResultAddress] == null )
            {
                ExpressionTarget rTarget = compilation.Parse( expr.Right );

                compilation.ConstValTypes[target.ResultAddress] = rTarget.ResultAddress;

                return target;
            }
            else
            {
                ExpressionTarget rTarget = compilation.Parse(
                                                             expr.Right
                                                            ).MakeAddress(compilation);

                List < string > lines = new List < string >();

                //lines.Add(
                //          $"COPY {rTarget.ResultAddress} {target.ResultAddress} ; Left: {expr.Left} ; Right: {expr.Right}"
                //         );
                if ( target.IsPointer )
                {
                    if ( rTarget.IsPointer )
                    {
                        lines.Add(
                                  $"CREF {rTarget.ResultAddress} {target.ResultAddress} ; Left: {expr.Left} ; Right: {expr.Right}"
                                 );
                    }
                    else
                    {
                        ExpressionTarget tmpTarget = new ExpressionTarget(
                                                                          compilation.GetTempVarLoad(
                                                                               rTarget.ResultAddress
                                                                              ),
                                                                          true,
                                                                          compilation.TypeSystem.GetType( "var" )
                                                                         );

                        lines.Add(
                                  $"CREF {tmpTarget.ResultAddress} {target.ResultAddress} ; Left: {expr.Left} ; Right: {expr.Right}"
                                 );

                        compilation.ReleaseTempVar( tmpTarget.ResultAddress );
                    }
                }
                else
                {
                    if ( rTarget.IsPointer )
                    {
                        ExpressionTarget tmpTarget = new ExpressionTarget(
                                                                          compilation.GetTempVarLoad(
                                                                               target.ResultAddress
                                                                              ),
                                                                          true,
                                                                          compilation.TypeSystem.GetType( "var" )
                                                                         );

                        lines.Add(
                                  $"CREF {rTarget.ResultAddress} {tmpTarget.ResultAddress} ; Left: {expr.Left} ; Right: {expr.Right}"
                                 );

                        compilation.ReleaseTempVar( tmpTarget.ResultAddress );
                    }
                    else if ( rTarget.ResultAddress != target.ResultAddress )
                    {
                        if ( !rTarget.IsAddress )
                        {
                            lines.Add(
                                      $"LOAD {target.ResultAddress} {rTarget.ResultAddress} ; Left: {expr.Left} ; Right: {expr.Right}"
                                     );
                        }
                        else
                        {
                            lines.Add(
                                      $"COPY {rTarget.ResultAddress} {target.ResultAddress} ; Left: {expr.Left} ; Right: {expr.Right}"
                                     );
                        }
                    }
                }

                compilation.ReleaseTempVar( rTarget.ResultAddress );
                compilation.ProgramCode.AddRange( lines );

                return target;
            }
        }

        #endregion

    }

}
