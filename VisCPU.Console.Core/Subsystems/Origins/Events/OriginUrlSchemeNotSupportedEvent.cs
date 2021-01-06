using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Console.Core.Subsystems.Origins.Events
{

    public class OriginUrlSchemeNotSupportedEvent : ErrorEvent
    {

        #region Public

        public OriginUrlSchemeNotSupportedEvent( string scheme ) : base(
                                                                        $"Scheme '{scheme}' is unsupported",
                                                                        ErrorEventKeys.ORIGIN_URL_SCHEME_UNSUPPORTED,
                                                                        false
                                                                       )
        {
        }

        #endregion

    }

}
