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
    public abstract class CombinedToken : IHlToken
    {
        /// <summary>
        ///     The Child Tokens
        /// </summary>
        public IHlToken[] SubTokens { get; }

        /// <summary>
        ///     Start index in the source
        /// </summary>
        public int SourceIndex { get; }

        /// <summary>
        ///     The Token FunctionType
        /// </summary>
        public HlTokenType Type { get; }

        #region Public

        /// <summary>
        ///     Returns the Child Tokens
        /// </summary>
        /// <returns></returns>
        public List < IHlToken > GetChildren()
        {
            return SubTokens?.ToList() ?? new List < IHlToken >();
        }

        /// <summary>
        ///     Returns the Value of this token
        /// </summary>
        /// <returns></returns>
        public string GetValue()
        {
            return Unpack( SubTokens );
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
        /// <param name="type">Token FunctionType</param>
        /// <param name="subtokens">Child Tokens</param>
        /// <param name="start">Start index in the source</param>
        protected CombinedToken( HlTokenType type, IHlToken[] subtokens, int start )
        {
            SubTokens = subtokens;
            SourceIndex = start;
            Type = type;
        }

        /// <summary>
        ///     Helper function that unpacks a Sequence of tokens into a string representation
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        protected static string Unpack( IHlToken[] t )
        {
            if ( t == null )
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();

            foreach ( IHlToken token in t )
            {
                sb.Append( token );
            }

            return sb.ToString();
        }

        #endregion
    }

}
