using System;

namespace VisCPU.Utility.IO.UriResolvers
{

    public class FileUriResolver : UriResolver
    {
        #region Protected

        protected override bool CanResolve( string uri )
        {
            if ( Uri.TryCreate( uri, UriKind.Absolute, out Uri u ) )
            {
                return u.Scheme == "file";
            }

            return false;
        }

        protected override string GetFilePath( string uri )
        {
            return uri;
        }

        protected override string Resolve( string uri )
        {
            string dst = GetFilePath( uri );
            Log( $"Resolving File: {uri} => {dst}" );

            return dst;
        }

        #endregion
    }

}
