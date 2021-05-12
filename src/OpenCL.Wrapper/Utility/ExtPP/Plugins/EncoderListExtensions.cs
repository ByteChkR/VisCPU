using System.Collections.Generic;
using System.Linq;

namespace Utility.ExtPP.Plugins
{

    public static class EncoderListExtensions
    {

        #region Public

        public static bool TryFindByKey(
            this List < TextEncoderPlugin.TextEncoding > list,
            string key,
            out TextEncoderPlugin.TextEncoding encoding )
        {
            encoding = list.FirstOrDefault( x => x.Key == key );

            return encoding != null;
        }

        #endregion

    }

}
