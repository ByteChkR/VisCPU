using System;

namespace Utility.ADL
{
    /// <summary>
    ///     Helpful functions are stored here.
    /// </summary>
    public static class Utils
    {

        public static readonly int BitsPerByte = 8;


        /// <summary>
        ///     Current Time Stamp based on DateTime.Now [hh:mm:ss]
        /// </summary>
        public static string TimeStamp => $"[{DateTime.Now:hh:mm:ss}]";

    }
}