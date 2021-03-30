using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisCPU.Console.Core;
using VisCPU.Utility;

namespace VisCPU.Console
{

    public static class Program
    {
        #region Private

        private static void Main( string[] args )
        {
            if ( args.Contains( "--root" ) )
            {
                int i = args.ToList().IndexOf( "--root" );
                AppRootHelper.SetCustomBase( Path.GetFullPath( args[i + 1] ) );
                List < string > a = new List < string >( args );
                a.RemoveRange( i, 2 );
                args = a.ToArray();
            }
            else
            {
                AppRootHelper.
                    SetAppDomainBase(); // Hack to be able to use the .netstandard libs in unity AND as console app.
            }

            VisConsole.RunConsole( args );
        }

        #endregion
    }

}
