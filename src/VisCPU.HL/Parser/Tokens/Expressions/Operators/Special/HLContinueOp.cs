﻿using System.Collections.Generic;

using VisCPU.HL.TypeSystem;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operators.Special
{

    /// <summary>
    ///     Continue Operator Implementation
    /// </summary>
    public class HlContinueOp : HlExpression
    {

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        public HlContinueOp( int sourceIdx ) : base( sourceIdx )
        {
        }

        /// <summary>
        ///     Returns Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List < IHlToken > GetChildren()
        {
            return new List < IHlToken >();
        }

        public override bool IsStatic()
        {
            return false;
        }

        #endregion

        public override HlTypeDefinition GetResultType( HlCompilation c )
        {
            return c.TypeSystem.GetType(c.Root, HLBaseTypeNames.s_UintTypeName);
        }

    }

}
