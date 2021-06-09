using System;
using System.Collections.Generic;

using VisCPU.HL.Compiler;
using VisCPU.HL.Compiler.Logic;
using VisCPU.HL.Compiler.Math.Assignments;
using VisCPU.HL.Compiler.Math.Atomic;
using VisCPU.HL.Compiler.Math.Bitwise;
using VisCPU.HL.Compiler.Math.Bitwise.Assignments;
using VisCPU.HL.Compiler.Math.Full;
using VisCPU.HL.Compiler.Memory;
using VisCPU.HL.Compiler.Relational;
using VisCPU.HL.Compiler.Special;
using VisCPU.HL.Compiler.Types;
using VisCPU.HL.Compiler.Variables;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL
{
    public abstract class HlRuntimeOperatorCompiler<T> where T: HlExpression
    {

        public abstract bool ContainsDefinition( HlCompilation compilation, T e );

        public abstract ExpressionTarget ParseExpression( HlCompilation compilation, T expr );

    }
    public class HlBinaryRuntimeOperatorCompiler : HlRuntimeOperatorCompiler<HlBinaryOp>
    {
        public override bool ContainsDefinition(HlCompilation compilation, HlBinaryOp e)
        {
            HlTypeDefinition tDef = e.GetResultType(compilation) ??
                                    compilation.TypeSystem.GetType(compilation.Root, HLBaseTypeNames.s_UintTypeName);
            return tDef.HasMember(e.Type.ToString());
        }

        public override ExpressionTarget ParseExpression(HlCompilation compilation, HlBinaryOp expr)
        {
            ExpressionTarget target = compilation.Parse(expr.Left);

            HlTypeDefinition tDef = target.TypeDefinition ??
                                    compilation.TypeSystem.GetType(compilation.Root, HLBaseTypeNames.s_UintTypeName);


            string funcName = tDef.GetFinalStaticFunction(expr.OperationType.ToString());

            return InvocationExpressionCompiler.ParseFunctionInvocation(
                                                                 compilation,
                                                                 new HlInvocationOp(
                                                                      new HlValueOperand(
                                                                           new HlTextToken(
                                                                                HlTokenType.OpWord,
                                                                                funcName,
                                                                                0
                                                                               )
                                                                          ),
                                                                      new[] { expr.Left, expr.Right }
                                                                     ),
                                                                 2,
                                                                 funcName,
                                                                 "JSR"
                                                                ).Cast((tDef.GetPrivateOrPublicMember(expr.OperationType.ToString()) as HlFunctionDefinition).ReturnType);

        }

    }
    public class HlUnaryRuntimeOperatorCompiler : HlRuntimeOperatorCompiler<HlUnaryOp>
    {
        public override bool ContainsDefinition(HlCompilation compilation, HlUnaryOp e)
        {
            HlTypeDefinition tDef = e.GetResultType(compilation) ??
                                    compilation.TypeSystem.GetType(compilation.Root, HLBaseTypeNames.s_UintTypeName);
            return tDef.HasMember(e.Type.ToString());
        }

        public override ExpressionTarget ParseExpression(HlCompilation compilation, HlUnaryOp expr)
        {
            ExpressionTarget target = compilation.Parse(expr.Left);

            HlTypeDefinition tDef = target.TypeDefinition ??
                                    compilation.TypeSystem.GetType(compilation.Root, HLBaseTypeNames.s_UintTypeName);


            string funcName = tDef.GetFinalStaticFunction(expr.OperationType.ToString());

            return InvocationExpressionCompiler.ParseFunctionInvocation(
                                                                 compilation,
                                                                 new HlInvocationOp(
                                                                      new HlValueOperand(
                                                                           new HlTextToken(
                                                                                HlTokenType.OpWord,
                                                                                funcName,
                                                                                0
                                                                               )
                                                                          ),
                                                                      new[] { expr.Left }
                                                                     ),
                                                                 1,
                                                                 funcName,
                                                                 "JSR"
                                                                );

        }

    }
    public class HlCompilerCollection
    {

        private readonly Dictionary < Type, IHlExpressionCompiler > m_TypeMap;

        #region Public

        public HlCompilerCollection( HlTypeSystem ts )
        {
            m_TypeMap = new Dictionary < Type, IHlExpressionCompiler >
                        {
                            { typeof( HlMemberAccessOp ), new MemberAccessCompiler() },
                            { typeof( HlVarDefOperand ), new VariableDefinitionExpressionCompiler( ts ) },
                            { typeof( HlArrayAccessorOp ), new ArrayAccessCompiler() },
                            { typeof( HlVarOperand ), new VarExpressionCompiler() },
                            { typeof( HlValueOperand ), new ConstExpressionCompiler() },
                            { typeof( HlInvocationOp ), new InvocationExpressionCompiler() },
                            { typeof( HlFuncDefOperand ), new FunctionDefinitionCompiler() },
                            { typeof( HlIfOp ), new IfBlockCompiler() },
                            { typeof( HlReturnOp ), new ReturnExpressionCompiler() },
                            { typeof( HlWhileOp ), new WhileExpressionCompiler() },
                            { typeof( HlForOp ), new ForExpressionCompiler() },
                            {
                                typeof( HlUnaryOp ), new OperatorCompilerCollection < HlUnaryOp
                                >(
                                  new Dictionary < HlTokenType,
                                      HlExpressionCompiler < HlUnaryOp > >
                                  {
                                      { HlTokenType.OpBang, new BoolNotExpressionCompiler() },
                                      { HlTokenType.OpUnaryPostfixIncrement, new PostfixIncrementExpressionCompiler() },
                                      { HlTokenType.OpUnaryPostfixDecrement, new PostfixDecrementExpressionCompiler() },
                                      { HlTokenType.OpUnaryPrefixIncrement, new PrefixIncrementExpressionCompiler() },
                                      { HlTokenType.OpUnaryPrefixDecrement, new PrefixDecrementExpressionCompiler() },
                                      { HlTokenType.OpReference, new ReferenceExpressionCompiler() },
                                      { HlTokenType.OpDeReference, new DereferenceExpressionCompiler() },
                                      { HlTokenType.OpTilde, new BitwiseInvertExpressionCompiler() },
                                      { HlTokenType.OpMinus, new UnaryMinusExpressionCompiler() }
                                  }, new HlUnaryRuntimeOperatorCompiler()
                                 )
                            },
                            {
                                typeof( HlBinaryOp ), new OperatorCompilerCollection <
                                    HlBinaryOp >(
                                                 new Dictionary < HlTokenType,
                                                     HlExpressionCompiler < HlBinaryOp
                                                     > >
                                                 {
                                                     { HlTokenType.OpEquality, new EqualExpressionCompiler() },
                                                     { HlTokenType.OpPlus, new AddExpressionCompiler() },
                                                     {
                                                         HlTokenType.OpMinus, new
                                                             SubExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpAsterisk, new
                                                             MulExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpComparison, new
                                                             EqualityExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpLogicalOr, new
                                                             BoolOrExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpLogicalAnd, new
                                                             BoolAndExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpPipe, new
                                                             BitwiseOrExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpAnd, new
                                                             BitwiseAndExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpPercent, new
                                                             ModExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpFwdSlash, new
                                                             DivExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpCap, new
                                                             BitwiseXOrExpressionCompiler()
                                                     },
                                                     { HlTokenType.OpLessThan, new LessThanExpressionCompiler() },
                                                     {
                                                         HlTokenType.OpGreaterThan, new
                                                             GreaterThanExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpLessOrEqual, new
                                                             LessEqualExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpGreaterOrEqual, new
                                                             GreaterEqualExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpShiftLeft, new
                                                             BitShiftLeftExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpShiftRight, new
                                                             BitShiftRightExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpSumAssign, new
                                                             AddAssignExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpDifAssign, new
                                                             SubAssignExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpProdAssign, new
                                                             MulAssignExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpQuotAssign, new
                                                             DivAssignExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpRemAssign, new
                                                             ModAssignExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpOrAssign, new
                                                             OrAssignExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpAndAssign, new
                                                             AndAssignExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpXOrAssign, new
                                                             XOrAssignExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpShiftLeftAssign, new
                                                             ShiftLeftAssignExpressionCompiler()
                                                     },
                                                     {
                                                         HlTokenType.OpShiftRightAssign, new
                                                             ShiftRightAssignExpressionCompiler()
                                                     }
                                                 }, new HlBinaryRuntimeOperatorCompiler()
                                                )
                            }
                        };
        }

        public bool ContainsCompiler( Type t )
        {
            return m_TypeMap.ContainsKey( t );
        }

        internal IHlExpressionCompiler Get( Type t )
        {
            return m_TypeMap[t];
        }

        #endregion

    }

}
