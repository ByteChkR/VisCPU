using System.Collections.Generic;

using VisCPU.HL.DataTypes;
using VisCPU.HL.TypeSystem;

/// <summary>
/// Contains XLangExpression Implementations for Operand Values.
/// </summary>
namespace VisCPU.HL.Parser.Tokens.Expressions.Operands
{

    /// <summary>
    ///     Implements a Variable Operand
    /// </summary>
    public class HlVarOperand : HlExpression
    {

        /// <summary>
        ///     The Token Value
        /// </summary>
        public virtual IHlToken Value { get; }

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="value">Variable Value</param>
        public HlVarOperand( IHlToken value, int sourceIdx ) : base( sourceIdx )
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
            return false;
        }

        public override HlTypeDefinition GetResultType( HlCompilation c )
        {
            if ( c.ContainsVariable( Value.ToString() ) )
            {
              VariableData data =   c.GetVariable( Value.ToString() );

              return data.TypeDefinition;
            }

            return null;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        #endregion

        #region Protected

        /// <summary>
        ///     Protected Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        protected HlVarOperand( int sourceIdx ) : base( sourceIdx )
        {
        }

        #endregion

    }

}
