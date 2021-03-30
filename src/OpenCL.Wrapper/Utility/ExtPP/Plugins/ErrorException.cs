using System;

namespace Utility.ExtPP.Plugins
{

    public class ErrorException : Exception
    {
        #region Public

        public ErrorException( string message ) : base( message )
        {
        }

        #endregion
    }

}
