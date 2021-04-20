using System.Collections.Generic;
using System.Linq;

using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Utility.IO.UriResolvers
{

    public abstract class UriResolver : VisBase
    {

        protected override LoggerSystems SubSystem => LoggerSystems.UriResolver;

        #region Public

        public static string GetFilePath( string tempPath, string uri )
        {
            UriResolver r = GetResolver( tempPath, uri );

            return r?.GetFilePath( uri );
        }

        public static string Resolve( string tempPath, string uri )
        {
            UriResolver r = GetResolver( tempPath, uri );

            return r?.Resolve( uri );
        }

        #endregion

        #region Protected

        protected abstract bool CanResolve( string uri );

        protected abstract string GetFilePath( string uri );

        protected abstract string Resolve( string uri );

        #endregion

        #region Private

        private static UriResolver GetResolver( string tempPath, string uri )
        {
            List < UriResolver > resolvers = new List < UriResolver >
                                             {
                                                 new HttpUriResolver( tempPath ),
                                                 new FileUriResolver()
                                             };

            UriResolver r = resolvers.FirstOrDefault( x => x.CanResolve( uri ) );

            return r;
        }

        #endregion

    }

}
