using System;

namespace VisCPU.Utility
{

    public static class AppRootHelper
    {

        public static string AppRoot { get; private set; }

        #region Public

        public static void SetAppDomainBase()
        {
            SetCustomBase( AppDomain.CurrentDomain.BaseDirectory );
        }

        public static void SetCustomBase( string baseDir )
        {
            AppRoot = baseDir;
        }

        #endregion

    }

}
