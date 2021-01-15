using System;

namespace VisCPU.Utility.Logging
{

    public static class Logger
    {

        public static readonly LoggerSettings s_Settings = new LoggerSettings();

        public static event Action < LoggerSystems, string > OnLogReceive;

        #region Public

        public static void LogMessage( LoggerSystems subsystem, string format, params object[] args )
        {
            if ( !s_Settings.EnableAll && ( s_Settings.EnabledSystems & subsystem ) == 0 )
            {
                return;
            }

            OnLogReceive?.Invoke( subsystem, string.Format( format, args ) );
        }

        #endregion

    }

}
