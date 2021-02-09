using System.Collections.Generic;

using VisCPU.HL.Parser.Events;
using VisCPU.HL.Parser.Tokens;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Parser
{

    /// <summary>
    ///     Collection of Tools for Parsing through a stream of tokens.
    /// </summary>
    public static class HlParsingTools
    {

        #region Public

        /// <summary>
        ///     Reads any symbol at the specified index.
        /// </summary>
        /// <param name="tokens">Token Stream</param>
        /// <param name="start">Start index</param>
        /// <returns>Token at index start</returns>
        public static IHlToken ReadAny( List < IHlToken > tokens, int start )
        {
            if ( !ReadAnyOrNone( tokens, start, out IHlToken ret ) )
            {
                EventManager < ErrorEvent >.SendEvent(
                                                      new HlTokenReadEvent( tokens, HlTokenType.Any, ret.Type, start )
                                                     );
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
        public static bool ReadAnyOrNone( List < IHlToken > tokens, int start, out IHlToken result )
        {
            if ( start >= 0 && tokens.Count > start )
            {
                result = tokens[start];

                return true;
            }

            result = new EofToken();

            return false;
        }

        public static int ReadList(
            List < IHlToken > tokens,
            int start,
            HlTokenType item,
            HlTokenType separator,
            out IHlToken[] items )
        {
            List < IHlToken > list = new List < IHlToken >();
            int i = start;

            while ( i < tokens.Count && tokens[i].Type == item )
            {
                list.Add( tokens[i] );

                i++;

                if ( i >= tokens.Count || tokens[i].Type != separator )
                {
                    items = list.ToArray();

                    return i - start;
                }

                i++;
            }

            EventManager < ErrorEvent >.SendEvent(
                                                  new HlTokenReadEvent( tokens, item, tokens[i].Type, start )
                                                 );

            items = list.ToArray();

            return i - start;
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
            List < IHlToken > tokens,
            int start,
            HlTokenType[] type,
            out IHlToken result )
        {
            foreach ( HlTokenType tokenType in type )
            {
                if ( ReadOneOrNone( tokens, start, tokenType, out result ) )
                {
                    return true;
                }

                if ( result.Type == HlTokenType.Eof )
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
        public static IHlToken[] ReadNoneOrMany( List < IHlToken > tokens, int start, int step, HlTokenType type )
        {
            List < IHlToken > ret = new List < IHlToken >();
            int currentStart = start;

            while ( ReadOneOrNone( tokens, currentStart, type, out IHlToken current ) )
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
        public static IHlToken[] ReadNoneOrManyOf( List < IHlToken > tokens, int start, int step, HlTokenType[] type )
        {
            List < IHlToken > res = new List < IHlToken >();
            int currentIdx = start;

            while ( true )
            {
                if ( ReadNoneOrAnyOf( tokens, currentIdx, type, out IHlToken found ) )
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
        public static IHlToken ReadOne( List < IHlToken > tokens, int start, HlTokenType type )
        {
            if ( !ReadOneOrNone( tokens, start, type, out IHlToken ret ) )
            {
                EventManager < ErrorEvent >.SendEvent( new HlTokenReadEvent( tokens, type, ret.Type, start ) );
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
        public static IHlToken ReadOneOfAny( List < IHlToken > tokens, int start, HlTokenType[] type )
        {
            if ( !ReadNoneOrAnyOf( tokens, start, type, out IHlToken ret ) )
            {
                EventManager < ErrorEvent >.SendEvent( new HlTokenReadEvent( tokens, type, ret.Type, start ) );
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
        public static IHlToken[] ReadOneOrMany( List < IHlToken > tokens, int start, int step, HlTokenType type )
        {
            List < IHlToken > ret = new List < IHlToken > { ReadOne( tokens, start, type ) };
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
        public static IHlToken[] ReadOneOrManyOf( List < IHlToken > tokens, int start, int step, HlTokenType[] type )
        {
            List < IHlToken > ret = new List < IHlToken > { ReadOneOfAny( tokens, start, type ) };
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
        public static bool ReadOneOrNone( List < IHlToken > tokens, int start, HlTokenType type, out IHlToken result )
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

            result = new EofToken();

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
        public static IHlToken[] ReadUntil( List < IHlToken > tokens, int start, int step, HlTokenType type )
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
        public static IHlToken[] ReadUntilAny( List < IHlToken > tokens, int start, int step, HlTokenType[] type )
        {
            List < IHlToken > ret = new List < IHlToken >();
            int currentStart = start;

            while ( true )
            {
                if ( ReadNoneOrAnyOf( tokens, currentStart, type, out IHlToken result ) )
                {
                    return ret.ToArray();
                }

                currentStart += step;

                if ( result.Type != HlTokenType.Eof )
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
