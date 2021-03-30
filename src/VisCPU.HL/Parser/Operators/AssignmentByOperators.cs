using VisCPU.HL.Parser.Events;
using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.HL.Parser.Tokens.Expressions.Operators;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Parser.Operators
{

    /// <summary>
    ///     Implements Assignment by Sum and Difference
    /// </summary>
    public class AssignmentByOperators : HlExpressionOperator
    {
        /// <summary>
        ///     Precedence Level of the Operators
        /// </summary>
        public override int PrecedenceLevel => 15;

        #region Public

        /// <summary>
        ///     Returns true if the parser is in a state that allows the creation of an implemented operator
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns>True if this Expression operator can create an expression</returns>
        public override bool CanCreate( HlExpressionParser parser, HlExpression currentNode )
        {
            return ( parser.CurrentToken.Type == HlTokenType.OpPlus ||
                     parser.CurrentToken.Type == HlTokenType.OpMinus ||
                     parser.CurrentToken.Type == HlTokenType.OpAsterisk ||
                     parser.CurrentToken.Type == HlTokenType.OpFwdSlash ||
                     parser.CurrentToken.Type == HlTokenType.OpPercent ||
                     parser.CurrentToken.Type == HlTokenType.OpAnd ||
                     parser.CurrentToken.Type == HlTokenType.OpPipe ||
                     parser.CurrentToken.Type == HlTokenType.OpCap ) &&
                   parser.Reader.PeekNext().Type == HlTokenType.OpEquality ||
                   parser.CurrentToken.Type == HlTokenType.OpLessThan &&
                   parser.Reader.PeekNext().Type == HlTokenType.OpLessThan &&
                   parser.Reader.PeekNext( 2 ).Type == HlTokenType.OpLessThan ||
                   parser.CurrentToken.Type == HlTokenType.OpGreaterThan &&
                   parser.Reader.PeekNext().Type == HlTokenType.OpGreaterThan &&
                   parser.Reader.PeekNext( 2 ).Type == HlTokenType.OpLessThan;
        }

        /// <summary>
        ///     Creates an implemented expression
        /// </summary>
        /// <param name="parser">XLExpressionParser</param>
        /// <param name="currentNode">Current Expression Node</param>
        /// <returns></returns>
        public override HlExpression Create( HlExpressionParser parser, HlExpression currentNode )
        {
            IHlToken token = parser.CurrentToken;

            HlTokenType tt;

            switch ( parser.CurrentToken.Type )
            {
                case HlTokenType.OpPlus:
                    tt = HlTokenType.OpSumAssign;

                    break;

                case HlTokenType.OpMinus:
                    tt = HlTokenType.OpDifAssign;

                    break;

                case HlTokenType.OpAsterisk:
                    tt = HlTokenType.OpProdAssign;

                    break;

                case HlTokenType.OpFwdSlash:
                    tt = HlTokenType.OpQuotAssign;

                    break;

                case HlTokenType.OpPercent:
                    tt = HlTokenType.OpRemAssign;

                    break;

                case HlTokenType.OpPipe:
                    tt = HlTokenType.OpOrAssign;

                    break;

                case HlTokenType.OpAnd:
                    tt = HlTokenType.OpAndAssign;

                    break;

                case HlTokenType.OpCap:
                    tt = HlTokenType.OpXOrAssign;

                    break;

                case HlTokenType.OpGreaterThan:
                    tt = HlTokenType.OpShiftRightAssign;
                    parser.Eat( token.Type );

                    break;

                case HlTokenType.OpLessThan:
                    tt = HlTokenType.OpShiftLeftAssign;
                    parser.Eat( token.Type );

                    break;

                default:
                    EventManager < ErrorEvent >.SendEvent(
                        new HlTokenReadEvent(
                            HlTokenType.Any,
                            parser.CurrentToken.Type
                        )
                    );

                    tt = HlTokenType.Unknown;

                    break;
            }

            parser.Eat( token.Type );
            parser.Eat( HlTokenType.OpEquality );

            return new HlBinaryOp( currentNode, tt, parser.ParseExpr( PrecedenceLevel ) );
        }

        #endregion
    }

}
