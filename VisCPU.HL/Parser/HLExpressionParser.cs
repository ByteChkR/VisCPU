using System.Collections.Generic;
using System.Linq;
using VisCPU.HL.Parser.Operators;
using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.Parser.Tokens.Expressions;

namespace VisCPU.HL.Parser
{
    /// <summary>
    ///     Parses XLangExpressions from a Token Stream
    /// </summary>
    public class HLExpressionParser
    {
        /// <summary>
        ///     Operator Collection
        /// </summary>
        private readonly HLExpressionOperatorCollection OpCollection;

        /// <summary>
        ///     Token Reader
        /// </summary>
        public readonly HLExpressionReader Reader;

        /// <summary>
        ///     Value Creator
        /// </summary>
        private readonly HLExpressionValueCreator ValueCreator;

        /// <summary>
        ///     The Current Token
        /// </summary>
        public IHLToken CurrentToken { get; private set; }

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="reader">Token Reader</param>
        /// <param name="valueCreator">XLExpression Value Creator</param>
        /// <param name="operators">Operator Collection</param>
        public HLExpressionParser(
            HLExpressionReader reader,
            HLExpressionValueCreator valueCreator,
            HLExpressionOperator[] operators)
        {
            OpCollection = new HLExpressionOperatorCollection(operators);
            ValueCreator = valueCreator;
            Reader = reader;
            CurrentToken = reader.GetNext();
        }

        /// <summary>
        ///     Creates a XLExpressionParser instance
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="reader">Expression Reader</param>
        /// <returns></returns>
        public static HLExpressionParser Create(HLExpressionReader reader)
        {
            HLExpressionOperator[] operators =
            {
                new ArraySubscriptOperator(),
                new AssignmentOperators(),
                new BitAndOperators(),
                new BitOrOperators(),
                new BitXOrOperators(),
                new InEqualityOperators(),
                new EqualityOperators(),
                new InvocationSelectorOperator(),
                new LogicalAndOperators(),
                new LogicalOrOperators(),
                new MemberSelectorOperator(),
                new MulDivModOperators(),
                new PlusMinusOperators(),
                new RelationOperators(),
                new UnaryOperators(),
                new AssignmentPlusMinusOperators(),
                new AssignmentByOperators()
            };

            HLExpressionValueCreator valueCreator = new HLExpressionValueCreator();

            return new HLExpressionParser(reader, valueCreator, operators);
        }

        /// <summary>
        ///     Consumes the Specified Token
        ///     Throws an Error if a different token was found
        /// </summary>
        /// <param name="type">Expected Token Type</param>
        public void Eat(HLTokenType type)
        {
            if (CurrentToken.Type == type)
            {
                CurrentToken = Reader.GetNext();
            }
            else
            {
                throw new HLTokenReadException(Reader.Tokens, type, CurrentToken.Type, CurrentToken.SourceIndex);
            }
        }

        /// <summary>
        ///     Parses the Expressions inside the Token Reader Stream
        /// </summary>
        /// <returns></returns>
        public HLExpression[] Parse()
        {
            if (CurrentToken.Type == HLTokenType.EOF)
            {
                return new HLExpression[0];
            }

            List<HLExpression> ret = new List<HLExpression> {ParseExpr(OpCollection.Lowest)};

            while (CurrentToken.Type != HLTokenType.EOF)
            {
                if (CurrentToken.Type == HLTokenType.OpSemicolon || CurrentToken.Type == HLTokenType.OpBlockToken)
                {
                    Eat(CurrentToken.Type);
                }

                if (CurrentToken.Type == HLTokenType.EOF)
                {
                    break;
                }

                ret.Add(ParseExpr(OpCollection.Lowest));
            }

            return ret.ToArray();
        }

        /// <summary>
        ///     Parses the Expression starting at the specified Operator Precedence
        /// </summary>
        /// <param name="startAt">Operator Precedence</param>
        /// <returns>Expression at the Specified Index</returns>
        public HLExpression ParseExpr(int startAt)
        {
            HLExpression node = ValueCreator.CreateValue(this);

            if (CurrentToken.Type == HLTokenType.OpSemicolon)
            {
                Eat(HLTokenType.OpSemicolon);

                return node;
            }

            for (int i = startAt; i <= OpCollection.Highest; i++)
            {
                if (!OpCollection.HasLevel(i))
                {
                    continue;
                }

                List<HLExpressionOperator> ops = OpCollection.GetLevel(i);

                HLExpressionOperator current = null;

                while ((current = ops.FirstOrDefault(x => x.CanCreate(this, node))) != null)
                {
                    node = current.Create(this, node);
                    i = startAt;
                }
            }

            return node;
        }

        #endregion
    }
}