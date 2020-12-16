namespace VisCPU.Utility.Logging
{

    public class LoggerSettings
    {

        [Argument(Name = "log-all")]
        public bool EnableAll;

        [Argument(Name = "log")]
        private LoggerSystems enabledSystems = LoggerSystems.Default;

        public LoggerSystems EnabledSystems => (LoggerSystems)(EnableAll ? -1 : (int)enabledSystems);


    }

}
