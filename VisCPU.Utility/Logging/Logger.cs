using System;

namespace VisCPU.Utility.Logging
{

    public static class Logger
    {

        public static readonly LoggerSettings Settings = new LoggerSettings();
        public static event Action<LoggerSystems, string> OnLogReceive;

        internal static void LogMessage(LoggerSystems subsystem, string message)
        {

            if (!Settings.EnableAll && (Settings.EnabledSystems & subsystem) == 0)
                return;

            OnLogReceive?.Invoke(subsystem, message);
        }

    }

}