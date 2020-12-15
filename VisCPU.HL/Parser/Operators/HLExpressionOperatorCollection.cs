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
        ///     Operators Sorted by Precedence
        /// </summary>
        private readonly Dictionary<int, PrecedenceBucket> buckets = new Dictionary<int, PrecedenceBucket>();

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="operators">Operators</param>
        public HLExpressionOperatorCollection(HLExpressionOperator[] operators)
        {
            foreach (HLExpressionOperator xLangExpressionOperator in operators)
            {
                if (buckets.ContainsKey(xLangExpressionOperator.PrecedenceLevel))
                {
                    buckets[xLangExpressionOperator.PrecedenceLevel].bucket.Add(xLangExpressionOperator);
                }
                else
                {
                    buckets[xLangExpressionOperator.PrecedenceLevel] =
                        new PrecedenceBucket(new List<HLExpressionOperator> { xLangExpressionOperator });
                }
            }
        }

        /// <summary>
        ///     The Highest Operator precedence
        /// </summary>
        public int Highest => buckets.Keys.Max();

        /// <summary>
        ///     The Lowest Operator precedence
        /// </summary>
        public int Lowest => buckets.Keys.Min();

        /// <summary>
        ///     Returns true if one or more operators exist with the specified precedence level
        /// </summary>
        /// <param name="level">Precedence Level</param>
        /// <returns></returns>
        public bool HasLevel(int level)
        {
            return buckets.ContainsKey(level);
        }

        /// <summary>
        ///     Returns all operators in a specific precedence level
        /// </summary>
        /// <param name="level">Precedence Level</param>
        /// <returns></returns>
        public List<HLExpressionOperator> GetLevel(int level)
        {
            return buckets[level].bucket;
        }

        /// <summary>
        ///     Precedence Bucket
        /// </summary>
        private struct PrecedenceBucket : IEquatable<PrecedenceBucket>
        {

            /// <summary>
            ///     Operators in this Precedence Bucket
            /// </summary>
            public readonly List<HLExpressionOperator> bucket;

            /// <summary>
            ///     Public Constructor
            /// </summary>
            /// <param name="operators">Operators</param>
            public PrecedenceBucket(List<HLExpressionOperator> operators)
            {
                bucket = operators;
            }

            public bool Equals(PrecedenceBucket other)
            {
                throw new NotImplementedException();
            }

        }

    }
}