using VisCPU.HL.Parser.Tokens.Combined;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operands
{

    /// <summary>
    ///     Implements a Variable Operand that is also a Variable Definition
    /// </summary>
    public class HLVarDefOperand : HLVarOperand
    {

        /// <summary>
        ///     The Definition Token
        /// </summary>
        public readonly VariableDefinitionToken value;

        public readonly HLExpression[] Initializer;

        /// <summary>
        ///     The Variable Value
        /// </summary>
        public override IHLToken Value => value.Name;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="value">Variable Value</param>
        public HLVarDefOperand( VariableDefinitionToken value, HLExpression[] initializer ) : base( value.SourceIndex )
        {
            Initializer = initializer;
            this.value = value;
        }

        public override string ToString()
        {
            return "def " + Value;
        }

        #endregion

    }

}
