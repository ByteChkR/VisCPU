using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Utility.UriResolvers.Events
{

    internal class UriResolverFailureEvent : ErrorEvent
    {

        #region Public

        public UriResolverFailureEvent( string uri ) : base(
                                                            $"Can not resolve uri '{uri}'",
                                                            ErrorEventKeys.s_UriResolverFailure,
                                                            true
                                                           )
        {
        }

        #endregion

    }

}
