using VisCPU.HL.Parser.Tokens.Combined;

namespace VisCPU.HL.Parser.Tokens.Expressions.Operands
{

    public class HLFuncDefOperand : HLVarOperand
    {

        public readonly HLExpression[] Block;

        /// <summary>
        ///     The Definition Token
        /// </summary>
        public readonly FunctionDefinitionToken value;

        /// <summary>
        ///     The Variable Value
        /// </summary>
        public override IHLToken Value => value;

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="value">Variable Value</param>
        public HLFuncDefOperand( FunctionDefinitionToken value, HLExpression[] block ) : base( value.SourceIndex )
        {
            this.value = value;
            Block = block;
        }

        public override string ToString()
        {
            return "fdef " + Value;
        }

        #endregion

    }

}
