using System.Collections.Generic;

namespace VisCPU.Utility.Extensions
{

    public static class ExtensionLoader
    {
        #region Public

        public static IEnumerable < T > LoadFrom < T >( string directory, bool recursive )
        {
            return new ExtensionLoadResult < T >( directory, recursive );
        }

        public static IEnumerable < T > LoadFrom < T >( string[] files )
        {
            return new ExtensionLoadResult < T >( files );
        }

        #endregion

    }

}
