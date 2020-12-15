﻿using System.Collections.Generic;
using System.Linq;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operators.Special
{
    /// <summary>
    ///     If/Else Operator Implementation
    /// </summary>
    public class HLIfOp : HLExpression
    {

        /// <summary>
        ///     Condition Map
        /// </summary>
        public readonly List<(HLExpression, HLExpression[])> ConditionMap;

        /// <summary>
        ///     Else Branch Block
        /// </summary>
        public readonly HLExpression[] ElseBranch;

        /// <summary>
        ///     Operation Type
        /// </summary>
        public readonly HLTokenType OperationType = HLTokenType.OpIf;

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="operationType">Operation Type</param>
        /// <param name="conditionMap">Condition Map</param>
        /// <param name="elseBranch">Else Branch Block</param>
        public HLIfOp(
            List<(HLExpression, HLExpression[])> conditionMap,
            HLExpression[] elseBranch, int sourceIdx) : base(sourceIdx)
        {
            ConditionMap = conditionMap;
            ElseBranch = elseBranch;
        }

        public override HLTokenType Type => OperationType;

        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List<IHLToken> GetChildren()
        {
            return ConditionMap.SelectMany(x => x.Item2.Concat(x.Item2)).Concat(ElseBranch).Cast<IHLToken>().ToList();
        }

    }
}