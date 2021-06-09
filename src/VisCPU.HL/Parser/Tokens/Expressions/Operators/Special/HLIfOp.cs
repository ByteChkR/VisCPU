using System.Collections.Generic;
using System.Linq;

using VisCPU.HL.TypeSystem;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operators.Special
{

    /// <summary>
    ///     If/Else Operator Implementation
    /// </summary>
    public class HlIfOp : HlExpression
    {

        /// <summary>
        ///     Condition Map
        /// </summary>
        public List < (HlExpression, HlExpression[]) > ConditionMap { get; }

        /// <summary>
        ///     Else Branch Block
        /// </summary>
        public HlExpression[] ElseBranch { get; }

        /// <summary>
        ///     Operation FunctionType
        /// </summary>
        public HlTokenType OperationType { get; } = HlTokenType.OpIf;

        public override HlTokenType Type => OperationType;

        public override HlTypeDefinition GetResultType( HlCompilation c )
        {
            return c.TypeSystem.GetType(c.Root, HLBaseTypeNames.s_UintTypeName);
        }

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="operationType">Operation FunctionType</param>
        /// <param name="conditionMap">Condition Map</param>
        /// <param name="elseBranch">Else Branch Block</param>
        public HlIfOp(
            List < (HlExpression, HlExpression[]) > conditionMap,
            HlExpression[] elseBranch,
            int sourceIdx ) : base( sourceIdx )
        {
            ConditionMap = conditionMap;
            ElseBranch = elseBranch;
        }

        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List < IHlToken > GetChildren()
        {
            return ConditionMap.SelectMany( x => x.Item2.Concat( x.Item2 ) ).
                                Concat( ElseBranch ).
                                Cast < IHlToken >().
                                ToList();
        }

        public override bool IsStatic()
        {
            return false;
        }

        #endregion

    }

}
