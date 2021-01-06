using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Utility.UriResolvers.Events
{

    public class UriResolverFailureEvent : ErrorEvent
    {

        #region Public

        public UriResolverFailureEvent( string uri ) : base(
                                                            $"Can not resolve uri '{uri}'",
                                                            ErrorEventKeys.URI_RESOLVER_FAILURE,
                                                            true
                                                           )
        {
        }

        #endregion

    }

}
