using System;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Utility.Settings.Events
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
