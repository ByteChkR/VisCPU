using System;

namespace Utility.FastString
{
    public static class FString
    {

        public static int FastIndexOf(string source, string pattern)
        {
            return FastIndexOf(ref source, ref pattern, 0, source.Length);
        }

        public static int FastIndexOf(ref string source, string pattern)
        {
            return FastIndexOf(ref source, ref pattern, 0, source.Length);
        }

        public static int FastIndexOf(ref string source, string pattern, int start)
        {
            return FastIndexOf(ref source, ref pattern, start, source.Length);
        }

        public static int FastIndexOf(ref string source, string pattern, int start, int count)
        {
            return FastIndexOf(ref source, ref pattern, start, count);
        }

        public static int FastIndexOf(ref string source, ref string pattern, int start, int count)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException();
            }

            if (pattern.Length == 0)
            {
                return 0;
            }

            if (pattern.Length == 1)
            {
                return source.IndexOf(pattern[0], start);
            }

            bool found;
            int limit = source.Length - pattern.Length + 1 - (source.Length - count);
            if (limit < 1)
            {
                return -1;
            }

            // Store the first 2 characters of "pattern"
            char c0 = pattern[0];
            char c1 = pattern[1];

            // Find the first occurrence of the first character
            int first = source.IndexOf(c0, start, limit);
            while (first != -1)
            {
                // Check if the following character is the same like
                // the 2nd character of "pattern"
                if (source[first + 1] != c1)
                {
                    first = source.IndexOf(c0, ++first, limit - first);
                    continue;
                }

                // Check the rest of "pattern" (starting with the 3rd character)
                found = true;
                for (int j = 2; j < pattern.Length; j++)
                {
                    if (source[first + j] == pattern[j])
                    {
                        continue;
                    }

                    found = false;
                    break;
                }

                // If the whole word was found, return its index, otherwise try again
                if (found)
                {
                    return first;
                }

                first = source.IndexOf(c0, ++first, limit - first);
            }

            return -1;
        }

    }
}