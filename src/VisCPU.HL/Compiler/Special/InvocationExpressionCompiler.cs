using System.Linq;

using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Compiler.Special.Compiletime;
using VisCPU.HL.DataTypes;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Special
{

    public class InvocationExpressionCompiler : HlExpressionCompiler < HlInvocationOp >
    {

        private readonly CompiletimeFunctionCompilerCollection m_CtFuncCollection =
            new CompiletimeFunctionCompilerCollection();

        #region Public

        public static ExpressionTarget ParseFunctionInvocation(
            HlCompilation compilation,
            HlInvocationOp expr,
            int targetLength,
            string functionName,
            string jumpInstruction )
        {
            if ( expr.ParameterList.Length != targetLength )
            {
                if ( !( expr.MemberDefinition != null &&
                        ( expr.MemberDefinition == expr.InstanceType.StaticConstructor ||
                          expr.MemberDefinition == expr.InstanceType.DynamicConstructor || 
                          expr.Instance==null ) ) )
                {
                    EventManager < ErrorEvent >.SendEvent(
                                                          new FunctionArgumentMismatchEvent(
                                                               $"Invalid parameter Count. Expected {targetLength} got {expr.ParameterList.Length}"
                                                              )
                                                         );
                }
            }

            if ( expr.Instance != null )
            {
                compilation.EmitterResult.Emit(
                                               $"PUSH",
                                               expr.Instance
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

            compilation.EmitterResult.Emit( jumpInstruction, functionName );

            ExpressionTarget tempReturn = new ExpressionTarget(
                                                               compilation.GetTempVarPop(),
                                                               true,
                                                               compilation.TypeSystem.GetType(compilation.Root,
                                                                    HLBaseTypeNames.s_UintTypeName
                                                                   )
                                                              );

            return tempReturn;
        }

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlInvocationOp expr )
        {
            string target = expr.Left.ToString();

            if ( m_CtFuncCollection.IsCompiletimeFunction( target ) )
            {
                return m_CtFuncCollection.Compile( target, compilation, expr );
            }

            bool isInternalFunc = compilation.FunctionMap.Contains( target );

            IExternalData externalSymbol =
                compilation.ExternalSymbols.FirstOrDefault(
                                                           x => x.GetName() == target &&
                                                                x.DataType == ExternalDataType.Function
                                                          );

            if ( compilation.TypeSystem.HasType(compilation.Root, target ) )
            {
                HlTypeDefinition tdef = compilation.TypeSystem.GetType(compilation.Root, target );
                string var = HlCompilation.GetUniqueName( "static_alloc" );
                uint size = tdef.GetSize();
                compilation.CreateVariable( var, size, tdef, false, false );
                string finalName = compilation.GetFinalName( var );

                ExpressionTarget ret = new ExpressionTarget(
                                                            compilation.GetTempVarLoad( finalName ),
                                                            true,
                                                            tdef,
                                                            true
                                                           );

                foreach ( HlFunctionDefinition tdefAbstractFunction in tdef.OverridableFunctions )
                {
                    HlFunctionDefinition test =
                        ( HlFunctionDefinition ) tdef.GetPrivateOrPublicMember( tdefAbstractFunction.Name );

                    if ( test.IsVirtual || test.IsOverride )
                    {
                        uint off = tdef.GetOffset( x => x == tdefAbstractFunction );

                        string tmp =
                            compilation.GetTempVarLoad( off.ToString() );

                        string implementingFunction = tdef.GetFinalMemberName( test );

                        string func =
                            compilation.GetTempVarLoad( implementingFunction );

                        compilation.EmitterResult.Emit( "ADD", tmp, ret.ResultAddress );
                        string tmpPtr = compilation.GetTempVarLoad( func );
                        compilation.EmitterResult.Emit( "CREF", tmpPtr, tmp );
                        compilation.ReleaseTempVar( tmp );
                        compilation.ReleaseTempVar( func );
                        compilation.ReleaseTempVar( tmpPtr );
                    }
                    else
                    {
                        EventManager < ErrorEvent >.SendEvent(
                                                              new MemberNotImplementedEvent(
                                                                   tdef,
                                                                   tdefAbstractFunction
                                                                  )
                                                             );
                    }
                }

                if ( tdef.StaticConstructor != null )
                {
                    expr.Redirect( ret.ResultAddress, tdef, tdef.StaticConstructor );

                    ParseExpression(
                                    compilation,
                                    expr
                                   );
                }

                return ret;
            }

            if ( expr.Instance != null &&
                 expr.MemberDefinition is HlFunctionDefinition fdef &&
                 ( fdef.IsVirtual || fdef.IsAbstract ) )
            {
                string init = expr.InstanceType.GetOffset( x => x == fdef ).ToString();
                string tmp = compilation.GetTempVarLoad( init );

                compilation.EmitterResult.Emit( "ADD", tmp, expr.Instance );
                compilation.EmitterResult.Emit( "DREF", tmp, tmp );
                int targetLength = fdef.ParameterTypes != null ? fdef.ParameterTypes.Length : expr.ParameterList.Length;

                ExpressionTarget t = ParseFunctionInvocation( compilation, expr, targetLength, tmp, "JSREF" );
                compilation.ReleaseTempVar( tmp );

                return t;
            }

            if ( isInternalFunc || externalSymbol != null )
            {
                string funcEmit = externalSymbol is LinkedData l ? l.Info.Address.ToString() : target;

                int targetLength = isInternalFunc
                                       ? compilation.FunctionMap.Get( target ).ParameterCount
                                       : ( externalSymbol as FunctionData )?.ParameterCount ??
                                         expr.ParameterList.Length;

                return ParseFunctionInvocation( compilation, expr, targetLength, funcEmit, "JSR" );
            }

            if ( compilation.ContainsVariable( target ) ||
                 compilation.ConstValTypes.Contains( target ) )
            {
                foreach ( HlExpression parameter in expr.ParameterList )
                {
                    ExpressionTarget tt = compilation.Parse( parameter ).MakeAddress( compilation );

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
                                                                   compilation.TypeSystem.GetType(compilation.Root,
                                                                        HLBaseTypeNames.s_UintTypeName
                                                                       )
                                                                  );

                return tempReturn;
            }

            EventManager < ErrorEvent >.SendEvent( new FunctionNotFoundEvent( target ) );

            return new ExpressionTarget();
        }

        #endregion

    }

}
