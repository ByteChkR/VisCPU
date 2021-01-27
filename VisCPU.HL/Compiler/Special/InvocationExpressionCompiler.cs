using System.Linq;
using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Compiler.Special.Compiletime;
using VisCPU.HL.DataTypes;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Special
{

    public class InvocationExpressionCompiler : HlExpressionCompiler < HlInvocationOp >
    {
        private readonly CompiletimeFunctionCompilerCollection m_CtFuncCollection =
            new CompiletimeFunctionCompilerCollection();

        #region Public

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlInvocationOp expr )
        {
            string target = expr.Left.ToString();

            if ( m_CtFuncCollection.IsCompiletimeFunction( target ) )
            {
                return m_CtFuncCollection.Compile( target, compilation, expr );
            }

            bool isInternalFunc = compilation.FunctionMap.ContainsKey( target );

            IExternalData externalSymbol =
                compilation.ExternalSymbols.FirstOrDefault(
                    x => x.GetName() == target &&
                         x.DataType == ExternalDataType.Function
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

                foreach ( HlExpression parameter in expr.ParameterList )
                {
                    ExpressionTarget arg = compilation.Parse(
                                                           parameter
                                                       ).
                                                       MakeAddress( compilation );

                    compilation.EmitterResult.Emit(
                        $"PUSH",
                        arg.ResultAddress
                    );

                    compilation.ReleaseTempVar( arg.ResultAddress );
                }

                compilation.EmitterResult.Emit( $"JSR", funcEmit );

                ExpressionTarget tempReturn = new ExpressionTarget(
                    compilation.GetTempVarPop(),
                    true,
                    compilation.TypeSystem.GetType( HLBaseTypeNames.s_UintTypeName )
                );

                return tempReturn;
            }

            if ( compilation.ContainsVariable( target ) ||
                 compilation.ConstValTypes.ContainsKey( target ) )
            {
                foreach ( HlExpression parameter in expr.ParameterList )
                {
                    ExpressionTarget tt = compilation.Parse( parameter );

                    compilation.EmitterResult.Emit(
                        $"PUSH",
                        tt.ResultAddress
                    );

                    compilation.ReleaseTempVar( tt.ResultAddress );
                }

                if ( compilation.ContainsVariable( target ) )
                {
                    compilation.EmitterResult.Emit( $"JSREF", compilation.GetFinalName( target ) );
                }
                else
                {
                    compilation.EmitterResult.Emit( $"JSREF", target );
                }

                ExpressionTarget tempReturn = new ExpressionTarget(
                    compilation.GetTempVarPop(),
                    true,
                    compilation.TypeSystem.GetType( HLBaseTypeNames.s_UintTypeName )
                );

                return tempReturn;
            }

            EventManager < ErrorEvent >.SendEvent( new FunctionNotFoundEvent( target ) );

            return new ExpressionTarget();
        }

        #endregion
    }

}
