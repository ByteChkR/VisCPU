using System;
using System.Collections.Generic;
using System.Linq;

namespace VisCPU.Utility
{

    public static class DictionaryExtensions
    {

        public static Dictionary<K, V> Filter<K, V>(this Dictionary <K, V> input,
                                                    Func<KeyValuePair<K, V>, bool> filter, bool createNew = true)
        {
            if ( createNew )
            {
                Dictionary <K, V> ret = new Dictionary <K, V>();

                foreach ( KeyValuePair <K, V> keyValuePair in input )
                {
                    if ( filter( keyValuePair ) )
                        ret[keyValuePair.Key] = keyValuePair.Value;
                }

                return ret;
            }
            else
            {
                foreach ( K s in input.Keys.ToArray() )
                {
                    if ( !filter( new KeyValuePair <K, V>( s, input[s] ) ) )
                        input.Remove( s );
                }

                return input;
            }
        }
        

    }

}