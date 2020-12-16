using System.Linq;

using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Events;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Compiler
{

    public class InvocationExpressionCompiler : HLExpressionCompiler < HLInvocationOp >
    {

        #region Public

        public override ExpressionTarget ParseExpression( HLCompilation compilation, HLInvocationOp expr )
        {
            string target = expr.Left.ToString();

            if ( target == "ptr_of" )
            {
                string v = compilation.GetTempVar();

                compilation.ProgramCode.Add(
                                            $"LOAD {v} {compilation.Parse( expr.ParameterList.First() ).ResultAddress}"
                                           );

                return new ExpressionTarget( v, true );
            }

            if ( target == "size_of" )
            {
                if ( expr.ParameterList.Length != 1 )
                {
                    EventManager < ErrorEvent >.SendEvent(
                                                          new FunctionArgumentMismatchEvent(
                                                               "Invalid Arguments. Expected size_of(variable)"
                                                              )
                                                         );
                }

                string v = compilation.GetTempVar();

                if ( !compilation.ContainsVariable( expr.ParameterList[0].ToString() ) )
                {
                    EventManager < ErrorEvent >.SendEvent(
                                                          new HLVariableNotFoundEvent(
                                                               expr.ParameterList[0].ToString(),
                                                               false
                                                              )
                                                         );
                }

                compilation.ProgramCode.Add(
                                            $"LOAD {v} {compilation.GetVariable( expr.ParameterList[0].ToString() ).Size}"
                                           );

                return new ExpressionTarget( v, true );
            }

            if ( target == "string" )
            {
                if ( expr.ParameterList.Length != 2 )
                {
                    EventManager < ErrorEvent >.SendEvent(
                                                          new FunctionArgumentMismatchEvent(
                                                               "Invalid Arguments. Expected string(varname, string value)"
                                                              )
                                                         );
                }

                string varName = expr.ParameterList[0].ToString();

                string content = expr.ParameterList[1].
                                      GetChildren().
                                      Select( x => x.ToString() ).
                                      Aggregate( ( input, elem ) => input + ' ' + elem );

                compilation.CreateVariable( varName, content );

                return new ExpressionTarget( compilation.GetFinalName( varName ), true );
            }

            if ( target == "val_of" )
            {
                string v = compilation.GetTempVar();

                compilation.ProgramCode.Add(
                                            $"DREF {compilation.Parse( expr.ParameterList.First() ).ResultAddress} {v}"
                                           );

                return new ExpressionTarget( v, true );
            }

            bool isInternalFunc = compilation.FunctionMap.ContainsKey( target );

            IExternalData externalSymbol =
                compilation.ExternalSymbols.FirstOrDefault(
                                                           x => x.GetName() == target &&
                                                                x.DataType == ExternalDataType.FUNCTION
                                                          );

            if ( isInternalFunc || externalSymbol != null )
            {
                int targetLength = isInternalFunc
                                       ? compilation.FunctionMap[target].ParameterCount
                                       : ( ( FunctionData ) externalSymbol ).ParameterCount;

                if ( expr.ParameterList.Length != targetLength )
                {
                    EventManager < ErrorEvent >.SendEvent(
                                                          new FunctionArgumentMismatchEvent(
                                                               $"Invalid parameter Count. Expected {targetLength} got {expr.ParameterList.Length}"
                                                              )
                                                         );
                }

                foreach ( HLExpression parameter in expr.ParameterList )
                {
                    ExpressionTarget arg = compilation.Parse( parameter ).MakeAddress( compilation );

                    compilation.ProgramCode.Add(
                                                $"PUSH {arg.ResultAddress}; Push Param {parameter}"
                                               );

                    compilation.ReleaseTempVar( arg.ResultAddress );
                }

                compilation.ProgramCode.Add( $"JSR {target}" );

                if ( isInternalFunc && compilation.FunctionMap[target].HasReturnValue ||
                     !isInternalFunc && ( ( FunctionData ) externalSymbol ).HasReturnValue )
                {
                    ExpressionTarget tempReturn = new ExpressionTarget( compilation.GetTempVar(), true );

                    compilation.ProgramCode.Add(
                                                $"; Write back return value to '{tempReturn.ResultAddress}'"
                                               );

                    compilation.ProgramCode.Add( $"POP {tempReturn.ResultAddress}" );

                    return tempReturn;
                }

                return new ExpressionTarget();
            }

            if ( compilation.ContainsVariable( target ) ||
                 compilation.ConstValTypes.ContainsKey( target ) )
            {
                foreach ( HLExpression parameter in expr.ParameterList )
                {
                    ExpressionTarget tt = compilation.Parse( parameter );

                    compilation.ProgramCode.Add(
                                                $"PUSH {tt.ResultAddress}; Push Param {parameter}"
                                               );

                    compilation.ReleaseTempVar( tt.ResultAddress );
                }

                if ( compilation.ContainsVariable( target ) )
                {
                    compilation.ProgramCode.Add( $"JSREF {compilation.GetFinalName( target )}" );
                }
                else
                {
                    compilation.ProgramCode.Add( $"JSREF {target}" );
                }

                return new ExpressionTarget();
            }

            EventManager < ErrorEvent >.SendEvent( new FunctionNotFoundEvent( target ) );

            return new ExpressionTarget();
        }

        #endregion

    }

}
