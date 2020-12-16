using VisCPU.Utility.Events;

namespace VisCPU.Utility
{

    public class UriResolverFailureEvent:ErrorEvent
    {

        private const string EVENT_KEY = "uri-resolver-failure";
        public UriResolverFailureEvent( string uri ) : base( $"Can not resolve uri '{uri}'", EVENT_KEY, true )
        {
        }

    }

}