using System;
using System.IO;
using System.Net;

namespace VisCPU.Utility
{

    public class HttpUriResolver : UriResolver
    {

        private readonly string tempPath;
        public HttpUriResolver(string tempPath)
        {
            this.tempPath = tempPath;
        }
        private readonly WebClient client = new WebClient();

        protected override bool CanResolve(string uri)
        {
            if (Uri.TryCreate(uri, UriKind.Absolute, out Uri u))
            {
                return u.Scheme == "http" || u.Scheme == "https";
            }

            return false;
        }

        protected override string Resolve( string uri )
        {
            string dst = GetFilePath(uri);
            Directory.CreateDirectory(tempPath);
            Log($"Resolving File: {uri} => {dst}");
            client.DownloadFile(uri, dst);
            return dst;
        }

        protected override string GetFilePath(string uri)
        {
            string name = Path.GetFileName(uri);
            return Path.Combine(tempPath, name);
        }

    }

}