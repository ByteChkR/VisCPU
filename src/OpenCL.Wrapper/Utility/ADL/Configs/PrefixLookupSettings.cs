using System;

namespace Utility.ADL.Configs
{
    [Flags]
    public enum PrefixLookupSettings
    {

        NoPrefix = 0,
        AddPrefixIfAvailable = 1,
        DeconstructMaskToFind = 2,
        OnlyOnePrefix = 4,
        BakePrefixes = 8

    }
}