using System.Collections.Generic;
using System.Linq;

namespace Utility.ExtPP.Base
{
    /// <summary>
    ///     The Extension class contains a multitude of useful operations on arrays and strings.
    /// </summary>
    public static class Extensions
    {

        /// <summary>
        ///     Creates a sub array starting from 0 to length
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="arr">Source array</param>
        /// <param name="length">length</param>
        /// <returns></returns>
        public static IEnumerable<T> SubArray<T>(this IEnumerable<T> arr, int length)
        {
            return SubArray(arr, 0, length);
        }


        /// <summary>
        ///     Creates a sub array starting from start to start+length
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="arr">Source array</param>
        /// <param name="start">start index</param>
        /// <param name="length">length</param>
        /// <returns></returns>
        public static IEnumerable<T> SubArray<T>(this IEnumerable<T> arr, int start, int length)
        {
            T[] ret = new T[length];
            T[] enumerable = arr as T[] ?? arr.ToArray();
            for (int i = start; i < start + length; i++)
            {
                ret.SetValue(enumerable.ElementAt(i), i - start);
            }

            return ret;
        }

    }
}