using System;

using VisCPU.Utility.Events;

namespace VisCPU.Utility.Settings
{

    public class SettingsLoaderNotFoundEvent : ErrorEvent
    {

        private const string EVENT_KEY = "settings-loader-not-found";

        public SettingsLoaderNotFoundEvent(Type targetType, bool canContinue = false) : base($"Could not find Settings Loader for Type: '{targetType.FullName}'", EVENT_KEY, canContinue)
        {
        }

    }

}