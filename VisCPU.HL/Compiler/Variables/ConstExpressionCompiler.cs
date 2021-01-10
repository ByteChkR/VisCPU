using VisCPU.HL.Parser.Tokens.Expressions.Operands;

namespace VisCPU.HL.Compiler.Variables
{

    public class ConstExpressionCompiler : HLExpressionCompiler < HLValueOperand >
    {

        protected override bool AllImplementations => true;

        #region Public

        public override ExpressionTarget ParseExpression( HLCompilation compilation, HLValueOperand expr )
        {
            ExpressionTarget tmp =
                new ExpressionTarget( expr.Value.ToString(), false, compilation.TypeSystem.GetType( "var" ) );

            return tmp;
        }

        public override ExpressionTarget ParseExpression(
            HLCompilation compilation,
            HLValueOperand expr,
            ExpressionTarget outputTarget )
        {
            compilation.ProgramCode.Add( $"LOAD {outputTarget.ResultAddress} {expr.Value}" );

            return outputTarget;
        }

        #endregion

    }

}
