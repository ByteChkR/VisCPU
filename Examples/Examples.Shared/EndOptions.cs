using System;

namespace Examples.Shared
{

    [Flags]
    public enum EndOptions
    {

        None = 0,
        CleanOutput = 1,
        CleanSource = 2,
        CleanInternal = 4,
        Default = CleanOutput | CleanInternal,

    }

}