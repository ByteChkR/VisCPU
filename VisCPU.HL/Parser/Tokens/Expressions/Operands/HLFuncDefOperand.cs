using VisCPU.HL.Parser.Tokens.Combined;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operands
{

    public class HLFuncDefOperand : HLVarOperand
    {

        public readonly HLExpression[] Block;

        /// <summary>
        ///     The Definition Token
        /// </summary>
        public readonly FunctionDefinitionToken FunctionDefinition;

        /// <summary>
        ///     The Variable Value
        /// </summary>
        public override IHlToken Value => FunctionDefinition;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="functionDefinition">Variable Value</param>
        public HLFuncDefOperand( FunctionDefinitionToken functionDefinition, HLExpression[] block ) :
            base( functionDefinition.SourceIndex )
        {
            FunctionDefinition = functionDefinition;
            Block = block;
        }

        public override string ToString()
        {
            return "fdef " + Value;
        }

        #endregion

    }

}
