using System;
using System.Collections.Generic;
using System.Linq;

using VisCPU.HL.Parser.Events;
using VisCPU.HL.Parser.Operators;
using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.Parser.Tokens.Expressions;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Parser
{

    /// <summary>
    ///     Parses XLangExpressions from a Token Stream
    /// </summary>
    public class HlExpressionParser
    {

        /// <summary>
        ///     Operator Collection
        /// </summary>
        private readonly HlExpressionOperatorCollection m_OpCollection;

        /// <summary>
        ///     Token Reader
        /// </summary>
        public HlExpressionReader Reader { get; }

        /// <summary>
        ///     Value Creator
        /// </summary>
        public HlExpressionValueCreator ValueCreator { get; }

        /// <summary>
        ///     The Current Token
        /// </summary>
        public IHlToken CurrentToken { get; private set; }

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="context">XL Context</param>
        /// <param name="reader">Token Reader</param>
        /// <param name="valueCreator">XLExpression Value Creator</param>
        /// <param name="operators">Operator Collection</param>
        public HlExpressionParser(
            HlExpressionReader reader,
            HlExpressionValueCreator valueCreator,
            HlExpressionOperator[] operators )
        {
            m_OpCollection = new HlExpressionOperatorCollection( operators );
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
        public static HlExpressionParser Create( HlExpressionReader reader )
        {
            HlExpressionOperator[] operators =
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
                new BitShiftOperators(),
                new AssignmentPlusMinusOperators(),
                new AssignmentByOperators()
            };

            HlExpressionValueCreator valueCreator = new HlExpressionValueCreator();

            return new HlExpressionParser( reader, valueCreator, operators );
        }

        /// <summary>
        ///     Consumes the Specified Token
        ///     Throws an Error if a different token was found
        /// </summary>
        /// <param name="type">Expected Token FunctionType</param>
        public void Eat( HlTokenType type )
        {
            if ( CurrentToken.Type == type )
            {
                CurrentToken = Reader.GetNext();
            }
            else
            {
                EventManager < ErrorEvent >.SendEvent(
                                                      new HlTokenReadEvent(
                                                                           Reader.Tokens,
                                                                           type,
                                                                           CurrentToken.Type,
                                                                           CurrentToken.SourceIndex
                                                                          )
                                                     );
            }
        }

        /// <summary>
        ///     Parses the Expressions inside the Token Reader Stream
        /// </summary>
        /// <returns></returns>
        public HlExpression[] Parse()
        {
            if ( CurrentToken.Type == HlTokenType.Eof )
            {
                return new HlExpression[0];
            }

            List < HlExpression > ret = new List < HlExpression > { ParseExpr( m_OpCollection.Highest ) };

            while ( CurrentToken.Type != HlTokenType.Eof )
            {
                if ( CurrentToken.Type == HlTokenType.OpSemicolon || CurrentToken.Type == HlTokenType.OpBlockToken )
                {
                    Eat( CurrentToken.Type );
                }

                if ( CurrentToken.Type == HlTokenType.Eof )
                {
                    break;
                }

                ret.Add( ParseExpr( m_OpCollection.Highest ) );
            }

            return ret.ToArray();
        }

        public HlExpression ParseExpr()
        {
            return ParseExpr( -1 );
        }

        /// <summary>
        ///     Parses the Expression starting at the specified Operator Precedence
        /// </summary>
        /// <param name="stopAt">Operator Precedence</param>
        /// <returns>Expression at the Specified Index</returns>
        public HlExpression ParseExpr( int stopAt )
        {
            if ( stopAt == -1 )
            {
                stopAt = m_OpCollection.Highest;
            }

            HlExpression node = ValueCreator.CreateValue( this, ( uint ) stopAt );

            if ( CurrentToken.Type == HlTokenType.OpSemicolon )
            {
                Eat( HlTokenType.OpSemicolon );

                return node;
            }

            int end = Math.Min( stopAt, m_OpCollection.Highest );

            for ( int i = 0; i <= end; i++ )
            {
                if ( !m_OpCollection.HasLevel( i ) )
                {
                    continue;
                }

                List < HlExpressionOperator > ops = m_OpCollection.GetLevel( i );
                HlExpressionOperator current = ops.FirstOrDefault( x => x.CanCreate( this, node ) );

                if ( current != null )
                {
                    node = current.Create( this, node );
                }
            }

            return node;
        }

        #endregion

    }

}
