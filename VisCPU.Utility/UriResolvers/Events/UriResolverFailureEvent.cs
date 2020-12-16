using VisCPU.Utility.Events;

namespace VisCPU.Utility.UriResolvers.Events
{

    public class UriResolverFailureEvent : ErrorEvent
    {

        private const string EVENT_KEY = "uri-resolver-failure";

        #region Public

        public UriResolverFailureEvent( string uri ) : base( $"Can not resolve uri '{uri}'", EVENT_KEY, true )
        {
        }

        #endregion

    }

}
