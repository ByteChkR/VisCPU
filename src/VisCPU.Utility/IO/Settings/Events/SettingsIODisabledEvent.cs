using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Utility.IO.Settings.Events
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
