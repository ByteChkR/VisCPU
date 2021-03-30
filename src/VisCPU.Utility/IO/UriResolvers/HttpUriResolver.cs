using System;
using System.IO;
using System.Net;

namespace VisCPU.Utility.IO.UriResolvers
{

    public class HttpUriResolver : UriResolver
    {
        private readonly WebClient m_Client = new WebClient();

        private readonly string m_TempPath;

        #region Public

        public HttpUriResolver( string tempPath )
        {
            m_TempPath = tempPath;
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

            return Path.Combine( m_TempPath, name );
        }

        protected override string Resolve( string uri )
        {
            string dst = GetFilePath( uri );
            Directory.CreateDirectory( m_TempPath );
            Log( $"Resolving File: {uri} => {dst}" );
            m_Client.DownloadFile( uri, dst );

            return dst;
        }

        #endregion
    }

}
