using VisCPU.Utility.ArgumentParser;

namespace VisCPU.Utility.Logging
{

    public class LoggerSettings
    {

        [field: Argument( Name = "log" )]
        private LoggerSystems m_EnabledSystems = LoggerSystems.Default;

        public void SetLogLevel(LoggerSystems systemFlags) => m_EnabledSystems = systemFlags;

        [field: Argument( Name = "log-all" )]
        public bool EnableAll { get; set; }

        public LoggerSystems EnabledSystems => ( LoggerSystems ) ( EnableAll ? -1 : ( int ) m_EnabledSystems );

    }

}
