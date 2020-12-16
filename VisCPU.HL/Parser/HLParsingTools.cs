using System.Collections.Generic;

using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.Parser
{

    /// <summary>
    ///     Collection of Tools for Parsing through a stream of tokens.
    /// </summary>
    public static class HLParsingTools
    {

        #region Public

        /// <summary>
        ///     Reads any symbol at the specified index.
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start index</param>
        /// <returns>Token at index start</returns>
        public static IHLToken ReadAny( List < IHLToken > tokens, int start )
        {
            if ( !ReadAnyOrNone( tokens, start, out IHLToken ret ) )
            {
                throw new HLTokenReadException( tokens, HLTokenType.Any, ret.Type, start );
            }

            return ret;
        }

        /// <summary>
        ///     Reads Any token or none if the token stream reached the end
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start Index</param>
        /// <param name="result">Specified Tokens</param>
        /// <returns></returns>
        public static bool ReadAnyOrNone( List < IHLToken > tokens, int start, out IHLToken result )
        {
            if ( start >= 0 && tokens.Count > start )
            {
                result = tokens[start];

                return true;
            }

            result = new EOFToken();

            return false;
        }

        /// <summary>
        ///     Reads None or any of the specified tokens
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start Index</param>
        /// <param name="type">Accepted Tokens</param>
        /// <param name="result">Read Token</param>
        /// <returns>True if any token was read.</returns>
        public static bool ReadNoneOrAnyOf(
            List < IHLToken > tokens,
            int start,
            HLTokenType[] type,
            out IHLToken result )
        {
            foreach ( HLTokenType tokenType in type )
            {
                if ( ReadOneOrNone( tokens, start, tokenType, out result ) )
                {
                    return true;
                }

                if ( result.Type == HLTokenType.EOF )
                {
                    return false;
                }
            }

            result = tokens[start];

            return false;
        }

        /// <summary>
        ///     Reads none or many of the specified token
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start Index</param>
        /// <param name="step">Step per read token</param>
        /// <param name="type">Accepted Type</param>
        /// <returns>Read Tokens</returns>
        public static IHLToken[] ReadNoneOrMany( List < IHLToken > tokens, int start, int step, HLTokenType type )
        {
            List < IHLToken > ret = new List < IHLToken >();
            int currentStart = start;

            while ( ReadOneOrNone( tokens, currentStart, type, out IHLToken current ) )
            {
                currentStart += step;
                ret.Add( current );
            }

            return ret.ToArray();
        }

        /// <summary>
        ///     Reads none or more tokens of the accepted tokens
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start Index</param>
        /// <param name="step">Step per read token</param>
        /// <param name="type">Accepted Types</param>
        /// <returns>Read Tokens</returns>
        public static IHLToken[] ReadNoneOrManyOf( List < IHLToken > tokens, int start, int step, HLTokenType[] type )
        {
            List < IHLToken > res = new List < IHLToken >();
            int currentIdx = start;

            while ( true )
            {
                if ( ReadNoneOrAnyOf( tokens, currentIdx, type, out IHLToken found ) )
                {
                    res.Add( found );
                    currentIdx += step;
                }
                else
                {
                    return res.ToArray();
                }
            }
        }

        /// <summary>
        ///     Reads Exactly one token of a specified type
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start Index</param>
        /// <param name="type">Accepted Type</param>
        /// <returns></returns>
        public static IHLToken ReadOne( List < IHLToken > tokens, int start, HLTokenType type )
        {
            if ( !ReadOneOrNone( tokens, start, type, out IHLToken ret ) )
            {
                throw new HLTokenReadException( tokens, type, ret.Type, start );
            }

            return ret;
        }

        /// <summary>
        ///     reads exactly one token of the specifed tokens.
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start index</param>
        /// <param name="type">Accepted Types</param>
        /// <returns>The read token</returns>
        public static IHLToken ReadOneOfAny( List < IHLToken > tokens, int start, HLTokenType[] type )
        {
            if ( !ReadNoneOrAnyOf( tokens, start, type, out IHLToken ret ) )
            {
                throw new HLTokenReadException( tokens, type, ret.Type, start );
            }

            return ret;
        }

        /// <summary>
        ///     Reads one or many tokens of the specified type
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start index</param>
        /// <param name="step">Step per read token</param>
        /// <param name="type">Accepted Token</param>
        /// <returns>Read Tokens</returns>
        public static IHLToken[] ReadOneOrMany( List < IHLToken > tokens, int start, int step, HLTokenType type )
        {
            List < IHLToken > ret = new List < IHLToken > { ReadOne( tokens, start, type ) };
            ret.AddRange( ReadNoneOrMany( tokens, start + step, step, type ) );

            return ret.ToArray();
        }

        /// <summary>
        ///     Reads one or more tokens of the specified types
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start index</param>
        /// <param name="step">Step per read token</param>
        /// <param name="type">Accepted Types</param>
        /// <returns>Read Tokens</returns>
        public static IHLToken[] ReadOneOrManyOf( List < IHLToken > tokens, int start, int step, HLTokenType[] type )
        {
            List < IHLToken > ret = new List < IHLToken > { ReadOneOfAny( tokens, start, type ) };
            ret.AddRange( ReadNoneOrManyOf( tokens, start + step, step, type ) );

            return ret.ToArray();
        }

        /// <summary>
        ///     reads one or none token of the accepted type
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start index</param>
        /// <param name="type">Token Type</param>
        /// <param name="result">Read Token</param>
        /// <returns>True if token was read.</returns>
        public static bool ReadOneOrNone( List < IHLToken > tokens, int start, HLTokenType type, out IHLToken result )
        {
            if ( start >= 0 && tokens.Count > start )
            {
                result = tokens[start];

                if ( tokens[start].Type == type )
                {
                    return true;
                }

                return false;
            }

            result = new EOFToken();

            return false;
        }

        /// <summary>
        ///     Reads until end of stream or the specified type has been read.
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start index</param>
        /// <param name="step">Step per read Token</param>
        /// <param name="type">End token type</param>
        /// <returns>Read Tokens</returns>
        public static IHLToken[] ReadUntil( List < IHLToken > tokens, int start, int step, HLTokenType type )
        {
            return ReadUntilAny( tokens, start, step, new[] { type } );
        }

        /// <summary>
        ///     Reads until any of the End Tokens were read.
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start index</param>
        /// <param name="step">Step per read Token</param>
        /// <param name="type">End Tokens</param>
        /// <returns>Read Tokens</returns>
        public static IHLToken[] ReadUntilAny( List < IHLToken > tokens, int start, int step, HLTokenType[] type )
        {
            List < IHLToken > ret = new List < IHLToken >();
            int currentStart = start;

            while ( true )
            {
                if ( ReadNoneOrAnyOf( tokens, currentStart, type, out IHLToken result ) )
                {
                    return ret.ToArray();
                }

                currentStart += step;

                if ( result.Type != HLTokenType.EOF )
                {
                    ret.Add( result );
                }
                else
                {
                    return ret.ToArray();
                }
            }
        }

        #endregion

    }

}
