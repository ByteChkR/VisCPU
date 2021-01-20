using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Console.Core.Subsystems.Origins.Events
{

    internal class OriginUrlSchemeNotSupportedEvent : ErrorEvent
    {
        #region Public

        public OriginUrlSchemeNotSupportedEvent( string scheme ) : base(
            $"Scheme '{scheme}' is unsupported",
            ErrorEventKeys.s_OriginUrlSchemeUnsupported,
            false
        )
        {
        }

        #endregion
    }

}
