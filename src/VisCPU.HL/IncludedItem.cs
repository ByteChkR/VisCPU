using System;

namespace VisCPU.HL
{

    public readonly struct IncludedItem : IEquatable < IncludedItem >
    {

        public readonly string Data;
        public readonly string[] ExternalSymbols;
        public readonly bool IsInline;
        public readonly bool IsLocal;

        public IncludedItem( string data, bool isInline, string[] externalSymbols = null, bool isLocal = false )
        {
            Data = data;
            IsInline = isInline;
            ExternalSymbols = externalSymbols;
            IsLocal = isLocal;
        }

        public bool Equals( IncludedItem other )
        {
            return Data == other.Data && IsInline == other.IsInline;
        }

        public override bool Equals( object obj )
        {
            return obj is IncludedItem other && Equals( other );
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ( ( Data != null ? Data.GetHashCode() : 0 ) * 397 ) ^ IsInline.GetHashCode();
            }
        }

    }

}
