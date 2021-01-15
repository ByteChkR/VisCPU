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
        public readonly VariableDefinitionToken VariableDefinition;

        public readonly HLExpression[] Initializer;

        /// <summary>
        ///     The Variable Value
        /// </summary>
        public override IHlToken Value => VariableDefinition.Name;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="variableDefinition">Variable Value</param>
        public HLVarDefOperand( VariableDefinitionToken variableDefinition, HLExpression[] initializer ) :
            base( variableDefinition.SourceIndex )
        {
            Initializer = initializer;
            VariableDefinition = variableDefinition;
        }

        public override string ToString()
        {
            return "def " + Value;
        }

        #endregion

    }

}
