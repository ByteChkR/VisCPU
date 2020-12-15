using System.Collections.Generic;

/// <summary>
/// Contains All Implemented Parser Tokens
/// </summary>
namespace VisCPU.HL.Parser.Tokens
{
    /// <summary>
    ///     Defines the Interface of a XLang Parsing Token
    /// </summary>
    public interface IHLToken
    {

        /// <summary>
        ///     The Token Type
        /// </summary>
        HLTokenType Type { get; }

        /// <summary>
        ///     The Start index in the source code.
        /// </summary>
        int SourceIndex { get; }

        /// <summary>
        ///     Returns the Child Tokens
        /// </summary>
        /// <returns>Child Tokens</returns>
        List<IHLToken> GetChildren();

    }
}