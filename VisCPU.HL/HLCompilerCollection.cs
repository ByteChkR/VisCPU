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
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;
using VisCPU.HL.TypeSystem;

namespace VisCPU.HL
{

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
                            { HlTokenType.OpUnaryIncrement, new IncrementExpressionCompiler() },
                            { HlTokenType.OpUnaryDecrement, new DecrementExpressionCompiler() },
                            { HlTokenType.OpReference, new ReferenceExpressionCompiler() },
                            { HlTokenType.OpDeReference, new DereferenceExpressionCompiler() },
                            { HlTokenType.OpTilde, new BitwiseInvertExpressionCompiler() },
                        }
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
                        }
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
