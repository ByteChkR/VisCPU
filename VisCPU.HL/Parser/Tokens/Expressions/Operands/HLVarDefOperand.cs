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


        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="value">Variable Value</param>
        public HLVarDefOperand(VariableDefinitionToken value) : base(value.SourceIndex)
        {
            this.value = value;
        }

        /// <summary>
        ///     The Variable Value
        /// </summary>
        public override IHLToken Value => value.Name;


        public override string ToString()
        {
            return "def " + Value;
        }

    }
}