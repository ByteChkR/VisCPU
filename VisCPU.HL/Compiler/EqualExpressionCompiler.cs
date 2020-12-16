using System.Collections.Generic;

using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Compiler
{
    public class EqualExpressionCompiler : HLExpressionCompiler<HLBinaryOp>
    {

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation, HLBinaryOp expr)
        {
            ExpressionTarget target = compilation.Parse(expr.Left);

            if (compilation.ConstValTypes.ContainsKey(target.ResultAddress) &&
                compilation.ConstValTypes[target.ResultAddress] == null)
            {
                ExpressionTarget rTarget = compilation.Parse(expr.Right);
                

                compilation.ConstValTypes[target.ResultAddress] = rTarget.ResultAddress;

                return target;
            }
            else
            {
                string rtName = compilation.GetTempVar();
                ExpressionTarget rTarget = compilation.Parse(
                                                             expr.Right, new ExpressionTarget(rtName, true));

                List<string> lines = new List<string>();
                //lines.Add(
                //          $"COPY {rTarget.ResultAddress} {target.ResultAddress} ; Left: {expr.Left} ; Right: {expr.Right}"
                //         );
                if (target.IsArray)
                {
                    if (rTarget.IsArray)
                    {
                        lines.Add(
                                  $"CREF {rTarget.ResultAddress} {target.ResultAddress} ; Left: {expr.Left} ; Right: {expr.Right}"
                                 );
                    }
                    else
                    {
                        ExpressionTarget tmpTarget = new ExpressionTarget(compilation.GetTempVar(), true);
                        lines.Add(
                                  $"LOAD {tmpTarget.ResultAddress} {rTarget.ResultAddress} ; Load Pointer for assignment"
                                 );
                        lines.Add(
                                  $"CREF {tmpTarget.ResultAddress} {target.ResultAddress} ; Left: {expr.Left} ; Right: {expr.Right}"
                                 );

                        compilation.ReleaseTempVar( tmpTarget.ResultAddress );
                    }
                }
                else
                {
                    if (rTarget.IsArray)
                    {
                        ExpressionTarget tmpTarget = new ExpressionTarget(compilation.GetTempVar(), true);
                        lines.Add(
                                  $"LOAD {tmpTarget.ResultAddress} {target.ResultAddress} ; Load Pointer for assignment"
                                 );
                        lines.Add(
                                  $"CREF {rTarget.ResultAddress} {tmpTarget.ResultAddress} ; Left: {expr.Left} ; Right: {expr.Right}"
                                 );
                        compilation.ReleaseTempVar(tmpTarget.ResultAddress);
                    }
                    else if (rTarget.ResultAddress != target.ResultAddress)
                    {
                        if (!rTarget.IsAddress)
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

                compilation.ReleaseTempVar( rtName );
                compilation.ProgramCode.AddRange(lines);

                return target;
            }
        }

    }
}