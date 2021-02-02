using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Utility.Settings.Events
{

    internal class SettingsIoDisabledEvent : ErrorEvent
    {
        #region Public

        public SettingsIoDisabledEvent( string errMessage ) : base(
            errMessage,
            ErrorEventKeys.s_SettingsIoDisabled,
            false
        )
        {
        }

        #endregion
    }

}
