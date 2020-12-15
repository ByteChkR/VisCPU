using System;
using System.Linq;

using VisCPU.Compiler.Assembler;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility.Events;

namespace VisCPU.HL.Compiler
{

    public class FunctionArgumentMismatchEvent : ErrorEvent
    {

        private const string EVENT_KEY = "func-arg-mismatch";
        public FunctionArgumentMismatchEvent(string errMessage) : base(errMessage, EVENT_KEY, false)
        {
        }

    }

    public class FunctionNotFoundEvent : ErrorEvent
    {

        private const string EVENT_KEY = "func-not-found";
        public FunctionNotFoundEvent(string funcName) : base($"Function '{funcName}' not found", EVENT_KEY, false)
        {
        }

    }

    public class InvocationExpressionCompiler : HLExpressionCompiler<HLInvocationOp>
    {

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation, HLInvocationOp expr)
        {
            string target = expr.Left.ToString();
            if (target == "ptr_of")
            {
                string v =  compilation.GetTempVar("ptr_of");
                compilation.ProgramCode.Add(
                                            $"LOAD {v} {compilation.Parse(expr.ParameterList.First()).ResultAddress}"
                                           );
                return new ExpressionTarget(v, true);
            }

            if (target == "size_of")
            {
                if (expr.ParameterList.Length != 1)
                {
                    EventManager < ErrorEvent >.SendEvent(
                                                          new FunctionArgumentMismatchEvent(
                                                               "Invalid Arguments. Expected size_of(variable)"
                                                              )
                                                         );
                }

                string v =   compilation.GetTempVar("size_of");
                if (!compilation.ContainsVariable(expr.ParameterList[0].ToString()))
                {
                    EventManager < ErrorEvent >.SendEvent(
                                                          new HLVariableNotFoundEvent(
                                                               expr.ParameterList[0].ToString(),
                                                               false
                                                              )
                                                         );
                }

                compilation.ProgramCode.Add(
                                            $"LOAD {v} {compilation.GetVariable(expr.ParameterList[0].ToString()).Size}"
                                           );
                return new ExpressionTarget(v, true);
            }

            if (target == "string")
            {
                if (expr.ParameterList.Length != 2)
                {
                    EventManager<ErrorEvent>.SendEvent(
                                                       new FunctionArgumentMismatchEvent(
                                                            "Invalid Arguments. Expected string(varname, string value)"
                                                           )
                                                      );
                }

                string varName = expr.ParameterList[0].ToString();
                string content = expr.ParameterList[1].GetChildren().Select(x => x.ToString())
                                     .Aggregate((input, elem) => input + ' ' + elem);
                compilation.CreateVariable(varName, content);


                return new ExpressionTarget(compilation.GetFinalName(varName), true);
            }

            if (target == "val_of")
            {
                string v =  compilation.GetTempVar("val_of");
                compilation.ProgramCode.Add(
                                            $"DREF {compilation.Parse(expr.ParameterList.First()).ResultAddress} {v}"
                                           );
                return new ExpressionTarget(v, true);
            }


            //target = compilation.Parse(expr.Left, possibleTarget).ResultAddress;

            bool isInternalFunc = compilation.FunctionMap.ContainsKey(target);
            IExternalData externalSymbol =
                compilation.ExternalSymbols.FirstOrDefault(
                                                           x => x.GetName() == target &&
                                                                x.DataType == ExternalDataType.FUNCTION
                                                          );
            if (isInternalFunc || externalSymbol != null)
            {
                int targetLength = isInternalFunc
                                       ? compilation.FunctionMap[target].ParameterCount
                                       : ((FunctionData) externalSymbol).ParameterCount;
                if (expr.ParameterList.Length != targetLength)
                {
                    EventManager < ErrorEvent >.SendEvent(
                                                          new FunctionArgumentMismatchEvent(
                                                               $"Invalid parameter Count. Expected {targetLength} got {expr.ParameterList.Length}"
                                                              )
                                                         );
                }

                foreach (HLExpression parameter in expr.ParameterList)
                {
                    ExpressionTarget arg = compilation.Parse(parameter).MakeAddress(compilation);
                    compilation.ProgramCode.Add(
                                                $"PUSH {arg.ResultAddress}; Push Param {parameter}"
                                               );
                }

                compilation.ProgramCode.Add($"JSR {target}");
                if (isInternalFunc && compilation.FunctionMap[target].HasReturnValue ||
                    !isInternalFunc && ((FunctionData) externalSymbol).HasReturnValue)
                {
                    ExpressionTarget tempReturn = new ExpressionTarget(compilation.GetTempVar("ret_" + target), true);
                    compilation.ProgramCode.Add(
                                                $"; Write back return value to '{tempReturn.ResultAddress}'"
                                               );
                    compilation.ProgramCode.Add($"POP {tempReturn.ResultAddress}");
                    return tempReturn;
                }


                return new ExpressionTarget();
            }

            if (compilation.ContainsVariable(target) ||
                compilation.ConstValTypes.ContainsKey(target))
            {
                foreach (HLExpression parameter in expr.ParameterList)
                {
                    ExpressionTarget tt = compilation.Parse(parameter);
                    compilation.ProgramCode.Add(
                                                $"PUSH {tt.ResultAddress}; Push Param {parameter}"
                                               );
                    compilation.ProgramCode.Add($"LOAD {tt.ResultAddress} 0x00; Reset Temp");
                }

                if (compilation.ContainsVariable(target))
                {
                    compilation.ProgramCode.Add($"JSREF {compilation.GetFinalName(target)}");
                }
                else
                {
                    compilation.ProgramCode.Add($"JSREF {target}");
                }


                return new ExpressionTarget();
            }

            EventManager < ErrorEvent >.SendEvent( new FunctionNotFoundEvent( target ));

            return new ExpressionTarget();
        }

    }
}