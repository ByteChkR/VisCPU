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

    public class HLCompilerCollection
    {

        private Dictionary < Type, IHLExpressionCompiler > TypeMap;

        #region Public

        public HLCompilerCollection( HLTypeSystem ts )
        {
            TypeMap = new Dictionary < Type, IHLExpressionCompiler >
                      {
                          { typeof( HLMemberAccessOp ), new MemberAccessCompiler() },
                          { typeof( HLVarDefOperand ), new VariableDefinitionExpressionCompiler( ts ) },
                          { typeof( HLArrayAccessorOp ), new ArrayAccessCompiler() },
                          { typeof( HLVarOperand ), new VarExpressionCompiler() },
                          { typeof( HLValueOperand ), new ConstExpressionCompiler() },
                          { typeof( HLInvocationOp ), new InvocationExpressionCompiler() },
                          { typeof( HLFuncDefOperand ), new FunctionDefinitionCompiler() },
                          { typeof( HLIfOp ), new IfBlockCompiler() },
                          { typeof( HLReturnOp ), new ReturnExpressionCompiler() },
                          { typeof( HLWhileOp ), new WhileExpressionCompiler() },
                          {
                              typeof( HLUnaryOp ), new OperatorCompilerCollection < HLUnaryOp
                              >(
                                new Dictionary < HLTokenType,
                                    HLExpressionCompiler < HLUnaryOp > >
                                {
                                    { HLTokenType.OpBang, new BoolNotExpressionCompiler() },
                                    { HLTokenType.OpUnaryIncrement, new IncrementExpressionCompiler() },
                                    { HLTokenType.OpUnaryDecrement, new DecrementExpressionCompiler() },
                                    { HLTokenType.OpReference, new ReferenceExpressionCompiler() },
                                    { HLTokenType.OpDeReference, new DereferenceExpressionCompiler() },
                                    { HLTokenType.OpTilde, new BitwiseInvertExpressionCompiler() },
                                }
                               )
                          },
                          {
                              typeof( HLBinaryOp ), new OperatorCompilerCollection <
                                  HLBinaryOp >(
                                               new Dictionary < HLTokenType,
                                                   HLExpressionCompiler < HLBinaryOp
                                                   > >
                                               {
                                                   { HLTokenType.OpEquality, new EqualExpressionCompiler() },
                                                   { HLTokenType.OpPlus, new AddExpressionCompiler() },
                                                   {
                                                       HLTokenType.OpMinus, new
                                                           SubExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpAsterisk, new
                                                           MulExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpComparison, new
                                                           EqualityExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpLogicalOr, new
                                                           BoolOrExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpLogicalAnd, new
                                                           BoolAndExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpPipe, new
                                                           BitwiseOrExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpAnd, new
                                                           BitwiseAndExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpPercent, new
                                                           ModExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpFwdSlash, new
                                                           DivExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpCap, new
                                                           BitwiseXOrExpressionCompiler()
                                                   },
                                                   { HLTokenType.OpLessThan, new LessThanExpressionCompiler() },
                                                   {
                                                       HLTokenType.OpGreaterThan, new
                                                           GreaterThanExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpLessOrEqual, new
                                                           LessEqualExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpGreaterOrEqual, new
                                                           GreaterEqualExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpShiftLeft, new
                                                           BitShiftLeftExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpShiftRight, new
                                                           BitShiftRightExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpSumAssign, new
                                                           AddAssignExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpDifAssign, new
                                                           SubAssignExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpProdAssign, new
                                                           MulAssignExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpQuotAssign, new
                                                           DivAssignExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpRemAssign, new
                                                           ModAssignExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpOrAssign, new
                                                           OrAssignExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpAndAssign, new
                                                           AndAssignExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpXOrAssign, new
                                                           XOrAssignExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpShiftLeftAssign, new
                                                           ShiftLeftAssignExpressionCompiler()
                                                   },
                                                   {
                                                       HLTokenType.OpShiftRightAssign, new
                                                           ShiftRightAssignExpressionCompiler()
                                                   }
                                               }
                                              )
                          }
                      };
        }

        public bool ContainsCompiler( Type t )
        {
            return TypeMap.ContainsKey( t );
        }

        internal IHLExpressionCompiler Get( Type t )
        {
            return TypeMap[t];
        }

        #endregion

    }

}
