using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Contains all Combined Tokens
/// </summary>
namespace VisCPU.HL.Parser.Tokens.Combined
{

    /// <summary>
    ///     Represents a Token that is constructed from its child tokens
    /// </summary>
    public abstract class CombinedToken : IHLToken
    {

        /// <summary>
        ///     The Child Tokens
        /// </summary>
        public readonly List < IHLToken > SubTokens;

        /// <summary>
        ///     Start index in the source
        /// </summary>
        public int SourceIndex { get; }

        /// <summary>
        ///     The Token Type
        /// </summary>
        public HLTokenType Type { get; }

        #region Public

        /// <summary>
        ///     Returns the Child Tokens
        /// </summary>
        /// <returns></returns>
        public List < IHLToken > GetChildren()
        {
            return SubTokens;
        }

        /// <summary>
        ///     Returns the Value of this token
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            return Unpack( SubTokens.ToArray() );
        }

        /// <summary>
        ///     String Representation of this Token
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetValue();
        }

        #endregion

        #region Protected

        /// <summary>
        ///     Protected Constructor
        /// </summary>
        /// <param name="type">Token Type</param>
        /// <param name="subtokens">Child Tokens</param>
        /// <param name="start">Start index in the source</param>
        protected CombinedToken( HLTokenType type, IHLToken[] subtokens, int start )
        {
            SubTokens = subtokens.ToList();
            SourceIndex = start;
            Type = type;
        }

        /// <summary>
        ///     Helper function that unpacks a Sequence of tokens into a string representation
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected static string Unpack( IHLToken[] t )
        {
            StringBuilder sb = new StringBuilder();

            foreach ( IHLToken token in t )
            {
                sb.Append( token );
            }

            return sb.ToString();
        }

        #endregion

    }

}
