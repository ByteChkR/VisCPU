using System;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Utility.IO.Settings.Events
{

    internal class SettingsLoaderNotFoundEvent : ErrorEvent
    {
        #region Public

        public SettingsLoaderNotFoundEvent( Type targetType, bool canContinue = false ) : base(
            $"Could not find SettingsSystem Loader for Type: '{targetType.FullName}'",
            ErrorEventKeys.s_SettingsLoaderNotFound,
            canContinue
        )
        {
        }

        #endregion
    }

}
