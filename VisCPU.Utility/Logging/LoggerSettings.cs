using VisCPU.Utility.ArgumentParser;

namespace VisCPU.Utility.Logging
{

    public class LoggerSettings
    {

        [Argument( Name = "log" )]
        private readonly LoggerSystems m_EnabledSystems = LoggerSystems.Default;

        [field: Argument( Name = "log-all" )]
        public bool EnableAll { get; }

        public LoggerSystems EnabledSystems => ( LoggerSystems ) ( EnableAll ? -1 : ( int ) m_EnabledSystems );

    }

}
