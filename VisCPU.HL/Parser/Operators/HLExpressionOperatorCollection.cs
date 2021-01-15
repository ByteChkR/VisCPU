using System;
using System.Collections.Generic;
using System.Linq;

namespace VisCPU.HL.Parser.Operators
{

    /// <summary>
    ///     A Collection of Expression Operators
    /// </summary>
    public class HLExpressionOperatorCollection
    {

        /// <summary>
        ///     Precedence Bucket
        /// </summary>
        private readonly struct PrecedenceBucket : IEquatable < PrecedenceBucket >
        {

            /// <summary>
            ///     Operators in this Precedence Bucket
            /// </summary>
            public readonly List < HLExpressionOperator > Bucket;

            /// <summary>
            ///     Public Constructor
            /// </summary>
            /// <param name="operators">Operators</param>
            public PrecedenceBucket( List < HLExpressionOperator > operators )
            {
                Bucket = operators;
            }

            public bool Equals( PrecedenceBucket other )
            {
                return Equals( Bucket, other.Bucket );
            }

            public override bool Equals( object obj )
            {
                return obj is PrecedenceBucket other && Equals( other );
            }

            public override int GetHashCode()
            {
                return Bucket != null ? Bucket.GetHashCode() : 0;
            }

        }

        /// <summary>
        ///     Operators Sorted by Precedence
        /// </summary>
        private readonly Dictionary < int, PrecedenceBucket > m_Buckets = new Dictionary < int, PrecedenceBucket >();

        /// <summary>
        ///     The Highest Operator precedence
        /// </summary>
        public int Highest => m_Buckets.Keys.Max();

        /// <summary>
        ///     The Lowest Operator precedence
        /// </summary>
        public int Lowest => m_Buckets.Keys.Min();

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="operators">Operators</param>
        public HLExpressionOperatorCollection( HLExpressionOperator[] operators )
        {
            foreach ( HLExpressionOperator xLangExpressionOperator in operators )
            {
                if ( m_Buckets.ContainsKey( xLangExpressionOperator.PrecedenceLevel ) )
                {
                    m_Buckets[xLangExpressionOperator.PrecedenceLevel].Bucket.Add( xLangExpressionOperator );
                }
                else
                {
                    m_Buckets[xLangExpressionOperator.PrecedenceLevel] =
                        new PrecedenceBucket( new List < HLExpressionOperator > { xLangExpressionOperator } );
                }
            }
        }

        /// <summary>
        ///     Returns all operators in a specific precedence level
        /// </summary>
        /// <param name="level">Precedence Level</param>
        /// <returns></returns>
        public List < HLExpressionOperator > GetLevel( int level )
        {
            return m_Buckets[level].Bucket;
        }

        /// <summary>
        ///     Returns true if one or more operators exist with the specified precedence level
        /// </summary>
        /// <param name="level">Precedence Level</param>
        /// <returns></returns>
        public bool HasLevel( int level )
        {
            return m_Buckets.ContainsKey( level );
        }

        #endregion

    }

}
