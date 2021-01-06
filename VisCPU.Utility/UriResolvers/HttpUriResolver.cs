using System;
using System.IO;
using System.Net;

namespace VisCPU.Utility.UriResolvers
{

    public class HttpUriResolver : UriResolver
    {

        private readonly WebClient client = new WebClient();

        private readonly string tempPath;

        #region Public

        public HttpUriResolver( string tempPath )
        {
            this.tempPath = tempPath;
        }

        #endregion

        #region Protected

        protected override bool CanResolve( string uri )
        {
            if ( Uri.TryCreate( uri, UriKind.Absolute, out Uri u ) )
            {
                return u.Scheme == "http" || u.Scheme == "https";
            }

            return false;
        }

        protected override string GetFilePath( string uri )
        {
            string name = Path.GetFileName( uri );

            return Path.Combine( tempPath, name );
        }

        protected override string Resolve( string uri )
        {
            string dst = GetFilePath( uri );
            Directory.CreateDirectory( tempPath );
            Log( $"Resolving File: {uri} => {dst}" );
            client.DownloadFile( uri, dst );

            return dst;
        }

        #endregion

    }

}
