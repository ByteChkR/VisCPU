using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Utility.IO.UriResolvers.Events
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
