using System.Linq;

using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Compiler.Special.Compiletime;
using VisCPU.HL.DataTypes;
using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.Parser.Tokens.Combined;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.IO.Settings;
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
                          expr.Instance == null ) ) )
                {
                    EventManager < ErrorEvent >.SendEvent(
                                                          new FunctionArgumentMismatchEvent(
                                                               $"{functionName}: Invalid parameter Count. Expected {targetLength} got {expr.ParameterList.Length}"
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
                                                               compilation.TypeSystem.GetType(
                                                                    compilation.Root,
                                                                    HLBaseTypeNames.s_UintTypeName
                                                                   )
                                                              );

            return tempReturn;
        }

        public static void WriteConstructorInvocationProlog(
            HlCompilation compilation,
            HlTypeDefinition tdef,
            string ret )
        {
            foreach ( HlFunctionDefinition tdefAbstractFunction in tdef.OverridableFunctions )
            {
                HlFunctionDefinition test =
                    ( HlFunctionDefinition ) tdef.GetPrivateOrPublicMember( tdefAbstractFunction.Name );

                if ( test.IsVirtual || test.IsOverride )
                {
                    uint off = tdef.GetOffset( tdefAbstractFunction.Name );

                    compilation.EmitterResult.Store(
                                                    $"; Applying Function Pointer: {test.Name} Offset from Begin: {off}"
                                                   );

                    string tmp =
                        compilation.GetTempVarLoad( off.ToString() );

                    string implementingFunction = tdef.GetFinalMemberName( test );
                    compilation.EmitterResult.Emit( "ADD", tmp, ret );

                    string instanceFuncPtr = tmp; //compilation.GetTempVarLoad( tmp );

                    //string check = compilation.GetTempVarDref( tmp );
                    //string endLbl = HlCompilation.GetUniqueName( $"{tdef.Name}_prolog" );
                    //compilation.EmitterResult.Emit( $"BNZ", check, endLbl );

                    string func =
                        compilation.GetTempVarLoad( implementingFunction );

                    string tmpPtr = compilation.GetTempVarLoad( func );
                    compilation.EmitterResult.Emit( "CREF", tmpPtr, instanceFuncPtr );
                    compilation.ReleaseTempVar( instanceFuncPtr );
                    compilation.ReleaseTempVar( func );
                    compilation.ReleaseTempVar( tmpPtr );

                    //compilation.EmitterResult.Store( $".{endLbl}" );
                }
                else if ( !tdef.IsAbstract )
                {
                    EventManager < ErrorEvent >.SendEvent(
                                                          new MemberNotImplementedErrorEvent(
                                                               tdef,
                                                               tdefAbstractFunction
                                                              )
                                                         );
                }
                else
                {
                    EventManager < WarningEvent >.SendEvent(
                                                            new MemberNotImplementedWarningEvent(
                                                                 tdef,
                                                                 tdefAbstractFunction
                                                                )
                                                           );
                }
            }
        }

        public static void WriteInlineConstructorInvocationProlog(
            HlCompilation compilation,
            HlTypeDefinition tdef,
            HlFuncDefOperand fdef )
        {
            for ( int i = fdef.FunctionDefinition.
                               Arguments.Length -
                          1;
                  i >= 0;
                  i-- )
            {
                IHlToken valueArgument = fdef.FunctionDefinition.
                                              Arguments[i];

                compilation.EmitterResult.Emit(
                                               $"POP",
                                               $"{compilation.GetFinalName( ( valueArgument as VariableDefinitionToken ).Name.ToString() )}"
                                              );
            }

            compilation.EmitterResult.Emit( $"POP", $"{compilation.GetFinalName( "this" )}" );

            WriteConstructorInvocationProlog(
                                             compilation,
                                             tdef,
                                             compilation.GetFinalName( "this" )
                                            );

            compilation.EmitterResult.Emit( $"PUSH", $"{compilation.GetFinalName( "this" )}" );

            for ( int i = 0;
                  i <
                  fdef.FunctionDefinition.
                       Arguments.Length;
                  i++ )
            {
                IHlToken valueArgument = fdef.FunctionDefinition.
                                              Arguments[i];

                compilation.EmitterResult.Emit(
                                               $"PUSH",
                                               $"{compilation.GetFinalName( ( valueArgument as VariableDefinitionToken ).Name.ToString() )}"
                                              );
            }
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

            //if (expr.Instance == null &&
            //    expr.MemberDefinition == null &&
            //    expr.InstanceType != null &&
            //    target == "new")
            //{
            //    ExpressionTarget instance = compilation.Parse(expr.ParameterList[0]);

            //    HlTypeDefinition tdef = expr.InstanceType;

            //    ExpressionTarget ret = new ExpressionTarget(
            //                                                instance.ResultAddress,
            //                                                true,
            //                                                tdef,
            //                                                true
            //                                               );

            //    if(SettingsManager.GetSettings<HlCompilerSettings>().ConstructorPrologMode == HlTypeConstructorPrologMode.Outline)
            //    WriteConstructorInvocationProlog(compilation, tdef, ret.ResultAddress);

            //    return ret;
            //}
            if ( expr.Instance == null &&
                 expr.InstanceType != null )
            {
                if ( expr.MemberDefinition == expr.InstanceType.StaticConstructor )
                {
                    ExpressionTarget instance = compilation.Parse( expr.ParameterList[0] );

                    HlTypeDefinition tdef = expr.InstanceType;

                    ExpressionTarget ret = new ExpressionTarget(
                                                                instance.ResultAddress,
                                                                true,
                                                                tdef,
                                                                true
                                                               );

                    if ( expr.WriteProlog &&
                         SettingsManager.GetSettings < HlCompilerSettings >().ConstructorPrologMode ==
                         HlTypeConstructorPrologMode.Outline )
                    {
                        WriteConstructorInvocationProlog( compilation, tdef, ret.ResultAddress );
                    }

                    expr.Redirect( ret.ResultAddress, tdef, tdef.StaticConstructor, expr.WriteProlog );

                    ParseExpression(
                                    compilation,
                                    expr
                                   );

                    return ret;
                }
                else
                {
                    int l = ( expr.MemberDefinition as HlFunctionDefinition ).ParameterTypes.Length;

                    return ParseFunctionInvocation( compilation, expr, l, target, "JSR" );
                }
            }

            if ( compilation.TypeSystem.HasType( compilation.Root, target ) )
            {
                string var = HlCompilation.GetUniqueName( "static_alloc" );
                HlTypeDefinition tdef = compilation.TypeSystem.GetType( compilation.Root, target );
                uint size = tdef.GetSize();
                compilation.CreateVariable( var, size, tdef, VariableDataEmitFlags.None );
                string finalName = compilation.GetFinalName( var );

                ExpressionTarget ret = new ExpressionTarget(
                                                            compilation.GetTempVarLoad( finalName ),
                                                            true,
                                                            tdef,
                                                            true
                                                           );

                if ( SettingsManager.GetSettings < HlCompilerSettings >().ConstructorPrologMode ==
                     HlTypeConstructorPrologMode.Outline )
                {
                    WriteConstructorInvocationProlog( compilation, tdef, ret.ResultAddress );
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
                 expr.MemberDefinition is HlFunctionDefinition fdef )
            {
                if ( fdef.IsVirtual || fdef.IsAbstract || fdef.IsOverride )
                {
                    uint i = expr.InstanceType.GetOffset( fdef.Name );

                    string init = i.ToString();
                    string tmp = compilation.GetTempVarLoad( init );

                    compilation.EmitterResult.Emit( "ADD", tmp, expr.Instance );
                    compilation.EmitterResult.Emit( "DREF", tmp, tmp );

                    int targetLength = fdef.ParameterTypes != null
                                           ? fdef.ParameterTypes.Length
                                           : expr.ParameterList.Length;

                    ExpressionTarget t = ParseFunctionInvocation( compilation, expr, targetLength, tmp, "JSREF" );
                    compilation.ReleaseTempVar( tmp );

                    return t;
                }
                else if ( !isInternalFunc && externalSymbol == null )
                {
                    string funcEmit = target;

                    int targetLength = fdef.ParameterTypes?.Length ?? expr.ParameterList.Length;

                    if ( expr.InstanceType != null &&
                         expr.InstanceType.StaticConstructor == expr.MemberDefinition &&
                         expr.WriteProlog &&
                         SettingsManager.GetSettings < HlCompilerSettings >().ConstructorPrologMode ==
                         HlTypeConstructorPrologMode.Inline )
                    {
                        funcEmit = expr.InstanceType.GetInternalConstructor( compilation );
                    }

                    return ParseFunctionInvocation( compilation, expr, targetLength, funcEmit, "JSR" );
                }
            }

            if ( isInternalFunc )
            {
                string funcEmit = target;

                int targetLength = compilation.FunctionMap.Get( target ).ParameterCount;

                //if (!expr.WriteProlog && SettingsManager.GetSettings < HlCompilerSettings >().ConstructorPrologMode ==
                //     HlTypeConstructorPrologMode.Inline )
                //{
                //    funcEmit = expr.MemberDefinition.
                //}

                if ( expr.InstanceType != null &&
                     expr.InstanceType.StaticConstructor == expr.MemberDefinition &&
                     expr.WriteProlog &&
                     SettingsManager.GetSettings < HlCompilerSettings >().ConstructorPrologMode ==
                     HlTypeConstructorPrologMode.Inline )
                {
                    funcEmit = expr.InstanceType.GetInternalConstructor( compilation );
                }

                return ParseFunctionInvocation( compilation, expr, targetLength, funcEmit, "JSR" );
            }

            if ( externalSymbol != null )
            {
                string funcEmit = ( externalSymbol as LinkedData )?.Info.Address.ToString() ?? target;

                int targetLength = ( externalSymbol as FunctionData )?.ParameterCount ?? expr.ParameterList.Length;

                if ( expr.InstanceType != null &&
                     expr.InstanceType.StaticConstructor == expr.MemberDefinition &&
                     expr.WriteProlog &&
                     SettingsManager.GetSettings < HlCompilerSettings >().ConstructorPrologMode ==
                     HlTypeConstructorPrologMode.Inline )
                {
                    funcEmit = expr.InstanceType.GetInternalConstructor( compilation );
                }

                return ParseFunctionInvocation( compilation, expr, targetLength, funcEmit, "JSR" );
            }

            //if (isInternalFunc || externalSymbol != null)
            //{
            //    string funcEmit = externalSymbol is LinkedData l ? l.Info.Address.ToString() : target;

            //    int targetLength = isInternalFunc
            //                           ? compilation.FunctionMap.Get(target).ParameterCount
            //                           : (externalSymbol as FunctionData)?.ParameterCount ??
            //                             expr.ParameterList.Length;

            //    //if (!expr.WriteProlog && SettingsManager.GetSettings < HlCompilerSettings >().ConstructorPrologMode ==
            //    //     HlTypeConstructorPrologMode.Inline )
            //    //{
            //    //    funcEmit = expr.MemberDefinition.
            //    //}

            //    return ParseFunctionInvocation(compilation, expr, targetLength, funcEmit, "JSR");
            //}

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
                                                                   compilation.TypeSystem.GetType(
                                                                        compilation.Root,
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
