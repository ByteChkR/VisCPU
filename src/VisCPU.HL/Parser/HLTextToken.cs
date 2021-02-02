﻿using System.Collections.Generic;
using VisCPU.HL.Parser.Tokens;

/// <summary>
/// Contains Base Token Implementations
/// </summary>
namespace VisCPU.HL.Parser
{

    /// <summary>
    ///     Represents a Token that contains a Sequence of Characters
    /// </summary>
    public class HlTextToken : IHlToken
    {
        /// <summary>
        ///     The Token Value
        /// </summary>
        public string Value { get; }

        /// <summary>
        ///     The Token type
        /// </summary>
        public HlTokenType Type { get; }

        /// <summary>
        ///     The Start index in the source stream
        /// </summary>
        public int SourceIndex { get; }

        #region Public

        /// <summary>
        ///     Public Constructor
        /// </summary>
        /// <param name="type">Token Type</param>
        /// <param name="value">Token Value</param>
        /// <param name="startIndex">Start index in the source stream</param>
        public HlTextToken( HlTokenType type, string value, int startIndex )
        {
            Type = type;
            Value = value;
            SourceIndex = startIndex;
        }

        /// <summary>
        ///     Returns all Children of this token
        /// </summary>
        /// <returns>Child Tokens</returns>
        public List < IHlToken > GetChildren()
        {
            return new List < IHlToken >();
        }

        /// <summary>
        ///     Returns the String Representation of this token
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value;
        }

        #endregion
    }

}
