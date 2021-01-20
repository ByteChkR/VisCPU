using VisCPU.HL.Parser.Tokens.Combined;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operands
{

    public class HlFuncDefOperand : HlVarOperand
    {
        public HlExpression[] Block { get; }

        /// <summary>
        ///     The Definition Token
        /// </summary>
        public FunctionDefinitionToken FunctionDefinition { get; }

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
        public HlFuncDefOperand( FunctionDefinitionToken functionDefinition, HlExpression[] block ) :
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
