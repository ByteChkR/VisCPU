using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens.Expressions.Operands;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Compiler.Variables
{

    public class ConstExpressionCompiler : HlExpressionCompiler < HlValueOperand >
    {
        protected override bool AllImplementations => true;

        #region Public

        public override ExpressionTarget ParseExpression( HlCompilation compilation, HlValueOperand expr )
        {
            string type;
            string value;
            switch ( expr.Value.Type )
            {
                case HlTokenType.OpCharLiteral:
                    type = HLBaseTypeNames.s_UintTypeName;
                    value = $"'{expr.Value}'";
                    break;
                case HlTokenType.OpDecimalNumber:
                    type = HLBaseTypeNames.s_FloatTypeName;

                    unsafe
                    {
                        float val = float.Parse(expr.Value.ToString());
                        value = (*(uint*)&val).ToString();
                    }
                    break;
                default:
                    type = HLBaseTypeNames.s_UintTypeName;
                    value = expr.Value.ToString();
                    break;

            }
            
            ExpressionTarget tmp =
                new ExpressionTarget( value, false, compilation.TypeSystem.GetType(type) );

            return tmp;
        }

        public override ExpressionTarget ParseExpression(
            HlCompilation compilation,
            HlValueOperand expr,
            ExpressionTarget outputTarget )
        {
            string type;
            string value;
            switch (expr.Value.Type)
            {
                case HlTokenType.OpCharLiteral:
                    type = HLBaseTypeNames.s_UintTypeName;
                    value = $"'{expr.Value}'";
                    break;
                case HlTokenType.OpDecimalNumber:
                    type = HLBaseTypeNames.s_FloatTypeName;

                    unsafe
                    {
                        float val = float.Parse( expr.Value.ToString() );
                        value = ( *( uint* ) &val ).ToString();
                    }
                    break;
                default:
                    type = HLBaseTypeNames.s_UintTypeName;
                    value = expr.Value.ToString();
                    break;

            }

            compilation.EmitterResult.Emit( $"LOAD", outputTarget.ResultAddress, value );

            return outputTarget.Cast(compilation.TypeSystem.GetType(type));
        }

        #endregion
    }

}
