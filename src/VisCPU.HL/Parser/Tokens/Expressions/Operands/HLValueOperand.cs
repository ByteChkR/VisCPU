﻿using System.Collections.Generic;

using VisCPU.HL.TypeSystem;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operands
{

    /// <summary>
    ///     Implements a (terminal) value expression
    /// </summary>
    public class HlValueOperand : HlExpression
    {

        /// <summary>
        ///     The Value
        /// </summary>
        public IHlToken Value { get; }

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">The XL Context</param>
        /// <param name="value">The Value of this Token</param>
        public HlValueOperand( IHlToken value ) : base( value.SourceIndex )
        {
            Value = value;
        }

        /// <summary>
        ///     Returns all Child Tokens of this Token
        /// </summary>
        /// <returns></returns>
        public override List < IHlToken > GetChildren()
        {
            return new List < IHlToken > { Value };
        }

        public override bool IsStatic()
        {
            return true;
        }

        public override HlTypeDefinition GetResultType( HlCompilation c )
        {
            return null;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        #endregion

    }

}
