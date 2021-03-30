using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Utility
{

    public static class UIntExtensions
    {
        #region Public

        public static IDictionary < string, AddressItem > ApplyOffset(
            this IDictionary < string, AddressItem > input,
            uint offset )
        {
            return input.ToDictionary( x => x.Key, x => x.Value.Offset( offset ) );
        }

        public static uint ParseHexUInt( this string val )
        {
            return uint.Parse( val.Remove( 0, 2 ), NumberStyles.HexNumber );
        }

        public static uint ParseUInt( this string val )
        {
            return val.StartsWith( "0x" ) ? ParseHexUInt( val ) : uint.Parse( val );
        }

        public static uint RawConvert( this float val )
        {
            unsafe
            {
                return *( uint* ) &val;
            }
        }

        public static float RawConvert( this uint val )
        {
            unsafe
            {
                return *( float* ) &val;
            }
        }

        public static byte[] ToBytes( this uint[] data )
        {
            return data.SelectMany( BitConverter.GetBytes ).ToArray();
        }

        public static string ToHexString( this uint val )
        {
            return "0x" + Convert.ToString( val, 16 );
        }

        public static uint[] ToUInt( this byte[] data )
        {
            uint[] programCode = new uint[data.Length / sizeof( uint )];

            for ( int i = 0; i < programCode.Length; i++ )
            {
                programCode[i] = BitConverter.ToUInt32( data, sizeof( uint ) * i );
            }

            return programCode;
        }

        public static bool TryParseHexUInt( this string val, out uint num )
        {
            if ( val.Length < 3 )
            {
                num = 0;

                return false;
            }

            return uint.TryParse( val.Remove( 0, 2 ), NumberStyles.HexNumber, new NumberFormatInfo(), out num );
        }

        public static bool TryParseUInt( this string val, out uint num )
        {
            return val.StartsWith( "0x" ) ? TryParseHexUInt( val, out num ) : uint.TryParse( val, out num );
        }

        #endregion
    }

}
