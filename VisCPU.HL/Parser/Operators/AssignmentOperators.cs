﻿using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;

namespace VisCPU.HL.Parser.Operators
{
    /// <summary>
    ///     Implements Assignment Operator
    /// </summary>
    public class AssignmentOperators : HLExpressionOperator
    {

        /// <summary>
        ///     Precedence Level of the Operators
        /// </summary>
        public override int PrecedenceLevel => 15;

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public override bool CanCreate(HLExpressionParser parser, HLExpression currentNode)
        {
            return parser.CurrentToken.Type == HLTokenType.OpEquality &&
                   parser.Reader.PeekNext().Type != HLTokenType.OpEquality;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override HLExpression Create(HLExpressionParser parser, HLExpression currentNode)
        {
            parser.Eat(HLTokenType.OpEquality);
            return new HLBinaryOp(currentNode, HLTokenType.OpEquality, parser.ParseExpr(0));
        }

    }
}