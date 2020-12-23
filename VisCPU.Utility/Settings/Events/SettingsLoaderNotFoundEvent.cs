using System;
using VisCPU.Utility.Events;

namespace VisCPU.Utility.Settings.Events
{
    public class SettingsLoaderNotFoundEvent : ErrorEvent
    {
        private const string EVENT_KEY = "settings-loader-not-found";

        #region Public

        public SettingsLoaderNotFoundEvent(Type targetType, bool canContinue = false) : base(
            $"Could not find SettingsSystem Loader for Type: '{targetType.FullName}'",
            EVENT_KEY,
            canContinue
        )
        {
        }

        #endregion
    }
}