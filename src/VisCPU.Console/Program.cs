using VisCPU.Console.Core;
using VisCPU.Utility;

namespace VisCPU.Console
{

    public static class Program
    {
        #region Private

        private static void Main( string[] args )
        {
            AppRootHelper.
                SetAppDomainBase(); // Hack to be able to use the .netstandard libs in unity AND as console app.

            VisConsole.RunConsole( args );
        }

        #endregion
    }

}
