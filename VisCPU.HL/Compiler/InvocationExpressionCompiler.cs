using System.Linq;

using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Events;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.HL.TypeSystem;
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
                ExpressionTarget et = compilation.Parse( expr.ParameterList.First() );

                string v = compilation.GetTempVar();

                compilation.ProgramCode.Add(
                                            $"LOAD {v} {et.ResultAddress}"
                                           );

                return new ExpressionTarget( v, true, compilation.TypeSystem.GetType( "var" ) );
            }

            if ( target == "static_cast" )
            {
                if ( expr.ParameterList.Length != 2 )
                {
                    EventManager < ErrorEvent >.SendEvent(
                                                          new FunctionArgumentMismatchEvent(
                                                               "Invalid Arguments. Expected static_cast(variable, type)"
                                                              )
                                                         );
                }

                return compilation.Parse( expr.ParameterList[0] ).
                                   Cast( compilation.TypeSystem.GetType( expr.ParameterList[1].ToString() ) );
            }

            if ( target == "offset_of" )
            {
                if ( expr.ParameterList.Length != 2 )
                {
                    EventManager < ErrorEvent >.SendEvent(
                                                          new FunctionArgumentMismatchEvent(
                                                               "Invalid Arguments. Expected offset_of(type, member)"
                                                              )
                                                         );
                }

                HLTypeDefinition type = compilation.TypeSystem.GetType( expr.ParameterList[0].ToString() );
                string v = compilation.GetTempVar();
                uint off = type.GetOffset( expr.ParameterList[1].ToString() );

                compilation.ProgramCode.Add(
                                            $"LOAD {v} {off}"
                                           );

                return new ExpressionTarget( v, true, compilation.TypeSystem.GetType( "var" ) );
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

                if ( compilation.ContainsVariable( expr.ParameterList[0].ToString() ) )
                {
                    compilation.ProgramCode.Add(
                                                $"LOAD {v} {compilation.GetVariable( expr.ParameterList[0].ToString() ).Size}"
                                               );

                    return new ExpressionTarget( v, true, compilation.TypeSystem.GetType( "var" ) );
                }

                if ( compilation.TypeSystem.HasType( expr.ParameterList[0].ToString() ) )
                {
                    compilation.ProgramCode.Add(
                                                $"LOAD {v} {compilation.TypeSystem.GetType( expr.ParameterList[0].ToString() ).GetSize()}"
                                               );

                    return new ExpressionTarget( v, true, compilation.TypeSystem.GetType( "var" ) );
                }

                EventManager < ErrorEvent >.SendEvent(
                                                      new HLVariableNotFoundEvent(
                                                           expr.ParameterList[0].ToString(),
                                                           false
                                                          )
                                                     );

                return new ExpressionTarget();
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

                compilation.CreateVariable( varName, content, compilation.TypeSystem.GetType( "var" ) );

                return new ExpressionTarget(
                                            compilation.GetFinalName( varName ),
                                            true,
                                            compilation.TypeSystem.GetType( "var" )
                                           );
            }

            if ( target == "val_of" )
            {
                string v = compilation.GetTempVar();
                ExpressionTarget t = compilation.Parse( expr.ParameterList.First() );

                compilation.ProgramCode.Add(
                                            $"DREF {t.ResultAddress} {v}"
                                           );

                return new ExpressionTarget( v, true, t.TypeDefinition );
            }

            bool isInternalFunc = compilation.FunctionMap.ContainsKey( target );

            IExternalData externalSymbol =
                compilation.ExternalSymbols.FirstOrDefault(
                                                           x => x.GetName() == target &&
                                                                x.DataType == ExternalDataType.FUNCTION
                                                          );

            if ( isInternalFunc || externalSymbol != null )
            {
                string funcEmit = externalSymbol is LinkedData l ? l.Info.Address.ToString() : target;

                int targetLength = isInternalFunc
                                       ? compilation.FunctionMap[target].ParameterCount
                                       : ( externalSymbol as FunctionData )?.ParameterCount ??
                                         expr.ParameterList.Length;

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

                    if ( arg.TypeDefinition.Name == "var" )
                    {
                        compilation.ProgramCode.Add(
                                                    $"PUSH {arg.ResultAddress}; Push Param {parameter}"
                                                   );
                    }
                    else
                    {
                        string v = compilation.GetTempVar();

                        compilation.ProgramCode.Add(
                                                    $"LOAD {v} {arg.ResultAddress}; Push Param {parameter}"
                                                   );

                        compilation.ProgramCode.Add(
                                                    $"PUSH {v}; Push Param {parameter}"
                                                   );

                        compilation.ReleaseTempVar( v );
                    }

                    compilation.ReleaseTempVar( arg.ResultAddress );
                }

                compilation.ProgramCode.Add( $"JSR {funcEmit}" );

                ExpressionTarget tempReturn = new ExpressionTarget(
                                                                   compilation.GetTempVar(),
                                                                   true,
                                                                   compilation.TypeSystem.GetType( "var" )
                                                                  );

                compilation.ProgramCode.Add(
                                            $"; Write back return value to '{tempReturn.ResultAddress}'"
                                           );

                compilation.ProgramCode.Add( $"POP {tempReturn.ResultAddress}" );

                return tempReturn;
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

                ExpressionTarget tempReturn = new ExpressionTarget(
                                                                   compilation.GetTempVar(),
                                                                   true,
                                                                   compilation.TypeSystem.GetType( "var" )
                                                                  );

                compilation.ProgramCode.Add( $"POP {tempReturn.ResultAddress}" );

                return tempReturn;
            }

            EventManager < ErrorEvent >.SendEvent( new FunctionNotFoundEvent( target ) );

            return new ExpressionTarget();
        }

        #endregion

    }

}
