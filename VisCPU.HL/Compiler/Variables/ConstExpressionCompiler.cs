using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;

namespace VisCPU.HL.Compiler.Variables
{

    public class ConstExpressionCompiler : HlExpressionCompiler < HlValueOperand >
    {
        protected override bool AllImplementations => true;

        #region Public

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlValueOperand expr )
        {
            string value = expr.Value.Type == HlTokenType.OpCharLiteral ? $"'{expr.Value}'" : expr.Value.ToString();

            ExpressionTarget tmp =
                new ExpressionTarget( value, false, compilation.TypeSystem.GetType( "var" ) );

            return tmp;
        }

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
            HlValueOperand expr,
            ExpressionTarget outputTarget )
        {
            string value = expr.Value.Type == HlTokenType.OpCharLiteral ? $"'{expr.Value}'" : expr.Value.ToString();

            compilation.EmitterResult.Emit( $"LOAD", outputTarget.ResultAddress, value );

            return outputTarget;
        }

        #endregion
    }

}
