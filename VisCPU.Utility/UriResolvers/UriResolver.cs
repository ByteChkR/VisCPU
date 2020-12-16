using System.Collections.Generic;
using System.Linq;

using VisCPU.Utility.Events;
using VisCPU.Utility.Logging;

namespace VisCPU.Utility
{

    public abstract class UriResolver : VisBase
    {
        public static string GetFilePath(string tempPath, string uri)
        {

            UriResolver r = GetResolver(tempPath, uri);

            if ( r != null )
                return r.GetFilePath( uri );
            return null;
        }
        
        public static void Resolve(string tempPath, string uri)
        {
            UriResolver r = GetResolver( tempPath, uri );

            if ( r != null )
            {
                r.Resolve(uri);
            }
        }

        private static UriResolver GetResolver( string tempPath, string uri )
        {
            List<UriResolver> resolvers = new List<UriResolver> { new HttpUriResolver(tempPath) };
            UriResolver r = resolvers.FirstOrDefault(x => x.CanResolve(uri));
            if (r != null)
            {
                return r;
            }
            return null;
        }

        protected override LoggerSystems SubSystem => LoggerSystems.UriResolver;

        protected abstract bool CanResolve(string uri);
        protected abstract string Resolve(string uri);
        protected abstract string GetFilePath(string uri);

    }

}