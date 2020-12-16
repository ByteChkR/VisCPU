using System;
using System.Linq;

using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.Parser.Tokens.Combined;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.HL.Parser.Tokens.Expressions.Operators.Special;

namespace VisCPU.HL.Parser
{
    /// <summary>
    ///     Implements AXLangExpressionValueCreator
    /// </summary>
    public class HLExpressionValueCreator
    {

        /// <summary>
        ///     Creates a Value based on the Current State of the Expression Parser
        /// </summary>
        /// <param name="parser">The Parser</param>
        /// <returns>Parsed Expression</returns>
        public HLExpression CreateValue(HLExpressionParser parser)
        {
            if (parser.CurrentToken.Type == HLTokenType.OpBang || parser.CurrentToken.Type == HLTokenType.OpTilde)
            {
                HLTokenType t = parser.CurrentToken.Type;
                parser.Eat(t);
                HLExpression token = new HLUnaryOp(CreateValue(parser), t);
                return token;
            }

            if (parser.CurrentToken.Type == HLTokenType.OpNew)
            {
                parser.Eat(parser.CurrentToken.Type);
                HLExpression token = new HLUnaryOp(parser.ParseExpr(0), HLTokenType.OpNew);
                return token;
            }

            if (parser.CurrentToken.Type == HLTokenType.OpReturn)
            {
                IHLToken rt = parser.CurrentToken;
                parser.Eat(HLTokenType.OpReturn);
                if (parser.CurrentToken.Type == HLTokenType.OpSemicolon)
                {
                    return new HLReturnOp(null, rt.SourceIndex);
                }

                return new HLReturnOp(parser.ParseExpr(0), rt.SourceIndex);
            }

            if (parser.CurrentToken.Type == HLTokenType.OpContinue)
            {
                IHLToken ct = parser.CurrentToken;
                parser.Eat(HLTokenType.OpContinue);
                return new HLContinueOp(ct.SourceIndex);
            }

            if (parser.CurrentToken.Type == HLTokenType.OpBreak)
            {
                IHLToken bt = parser.CurrentToken;
                parser.Eat(HLTokenType.OpBreak);
                return new HLBreakOp(bt.SourceIndex);
            }

            if (parser.CurrentToken.Type == HLTokenType.OpIf)
            {
                return HLSpecialOps.ReadIf(parser);
            }

            if (parser.CurrentToken.Type == HLTokenType.OpFor)
            {
                return HLSpecialOps.ReadFor(parser);
            }

            if (parser.CurrentToken.Type == HLTokenType.OpWhile)
            {
                return HLSpecialOps.ReadWhile(parser);
            }

            if (parser.CurrentToken.Type == HLTokenType.OpBracketOpen)
            {
                parser.Eat(HLTokenType.OpBracketOpen);
                HLExpression token = parser.ParseExpr(0);
                parser.Eat(HLTokenType.OpBracketClose);
                return token;
            }

            if (parser.CurrentToken.Type == HLTokenType.OpThis ||
                parser.CurrentToken.Type == HLTokenType.OpBase)
            {
                HLExpression token = new HLVarOperand(
                                                      parser.CurrentToken,
                                                      parser.CurrentToken.SourceIndex
                                                     );
                parser.Eat(parser.CurrentToken.Type);

                return token;
            }

            if (parser.CurrentToken.Type == HLTokenType.OpWord)
            {
                IHLToken item = parser.CurrentToken;
                HLExpression token = null;
                parser.Eat(parser.CurrentToken.Type);
                if (parser.CurrentToken.Type == HLTokenType.OpWord)
                {
                    IHLToken name = parser.CurrentToken;
                    parser.Eat(parser.CurrentToken.Type);
                    IHLToken num = null;

                    if (parser.CurrentToken.Type == HLTokenType.OpIndexerBracketOpen)
                    {
                        parser.Eat(parser.CurrentToken.Type);
                        num = parser.CurrentToken;
                        parser.Eat(HLTokenType.OpNumber);
                        parser.Eat(HLTokenType.OpIndexerBracketClose);
                    }


                    token = new HLVarDefOperand(
                                                new VariableDefinitionToken(
                                                                            name,
                                                                            item,
                                                                            new IHLToken[0],
                                                                            new IHLToken[0],
                                                                            null,
                                                                            num
                                                                           )
                                               );
                }
                else
                {
                    token = new HLVarOperand(item, item.SourceIndex);
                }

                return token;
            }


            if (parser.CurrentToken.Type == HLTokenType.OpVariableDefinition)
            {
                HLExpression token =
                    new HLVarDefOperand((VariableDefinitionToken) parser.CurrentToken);
                parser.Eat(HLTokenType.OpVariableDefinition);
                return token;
            }

            if (parser.CurrentToken.Type == HLTokenType.OpFunctionDefinition)
            {
                FunctionDefinitionToken fToken = (FunctionDefinitionToken) parser.CurrentToken;
                parser.Eat(HLTokenType.OpFunctionDefinition);
                HLExpression token =
                    new HLFuncDefOperand(
                                         fToken,
                                         HLExpressionParser.Create(new HLExpressionReader(fToken.Block.ToList()))
                                                           .Parse()
                                        );
                return token;
            }

            if (parser.CurrentToken.Type == HLTokenType.OpNumber ||
                parser.CurrentToken.Type == HLTokenType.OpStringLiteral)
            {
                HLExpression token = new HLValueOperand(parser.CurrentToken);
                parser.Eat(parser.CurrentToken.Type);
                return token;
            }


            throw new Exception("Invalid Token: " + parser.CurrentToken.Type+ "\n"+parser.Reader);
        }

    }
}