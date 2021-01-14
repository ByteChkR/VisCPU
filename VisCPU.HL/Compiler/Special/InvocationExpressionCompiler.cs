using System.Linq;

using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Compiler.Special.Compiletime;
using VisCPU.HL.DataTypes;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Compiler.Special
{

    public class InvocationExpressionCompiler : HLExpressionCompiler < HLInvocationOp >
    {

        private readonly CompiletimeFunctionCompilerCollection ctFuncCollection =
            new CompiletimeFunctionCompilerCollection();

        #region Public

        public override ExpressionTarget ParseExpression( HLCompilation compilation, HLInvocationOp expr )
        {
            string target = expr.Left.ToString();

            if ( ctFuncCollection.IsCompiletimeFunction( target ) )
            {
                return ctFuncCollection.Compile( target, compilation, expr );
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
                    ExpressionTarget arg = compilation.Parse(
                                                             parameter
                                                            ).
                                                       MakeAddress( compilation );

                    compilation.ProgramCode.Add(
                                                $"PUSH {arg.ResultAddress}; Push Param {parameter}"
                                               );

                    compilation.ReleaseTempVar( arg.ResultAddress );
                }

                compilation.ProgramCode.Add( $"JSR {funcEmit}" );

                ExpressionTarget tempReturn = new ExpressionTarget(
                                                                   compilation.GetTempVarPop(),
                                                                   true,
                                                                   compilation.TypeSystem.GetType( "var" )
                                                                  );

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
                                                                   compilation.GetTempVarPop(),
                                                                   true,
                                                                   compilation.TypeSystem.GetType( "var" )
                                                                  );

                return tempReturn;
            }

            EventManager < ErrorEvent >.SendEvent( new FunctionNotFoundEvent( target ) );

            return new ExpressionTarget();
        }

        #endregion

    }

}
