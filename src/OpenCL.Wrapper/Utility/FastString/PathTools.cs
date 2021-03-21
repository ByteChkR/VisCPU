using System.Collections.Generic;
using System.Linq;

namespace Utility.FastString
{
    public static class PathTools
    {

        public static string CleanPath(string path)
        {
            List<string> p = path.Replace("\\", "/").Split('/').ToList();
            for (int i = p.Count - 1; i >= 0; i--)
            {
                if (p[i] == "..")
                {
                    p.RemoveAt(i); //Remove the ..
                    if (i != 0)
                    {
                        if (p[i - 1] != "..")
                        {
                            p.RemoveAt(i - 1); //Remove the previous part
                        }
                        else
                        {
                            int remamount = 1;
                            for (int j = i - 1; j > 0; j--)
                            {
                                if (p[j] == "..")
                                {
                                    remamount++;
                                    p.RemoveAt(j);
                                }
                                else
                                {
                                    for (int k = j; k > j - remamount; k--)
                                    {
                                        p.RemoveAt(k);
                                    }

                                    i -= remamount;
                                    break;
                                }
                            }
                        }
                    }

                    i--;
                    continue;
                }

                if (p[i] == ".")
                {
                    p.RemoveAt(i);
                }
            }

            return p.Unpack("/");
        }

    }
}