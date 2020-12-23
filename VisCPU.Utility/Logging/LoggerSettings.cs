using VisCPU.Utility.ArgumentParser;

namespace VisCPU.Utility.Logging
{
    public class LoggerSettings
    {
        [Argument(Name = "log")] private readonly LoggerSystems enabledSystems = LoggerSystems.Default;

        [Argument(Name = "log-all")] public bool EnableAll;

        public LoggerSystems EnabledSystems => (LoggerSystems) (EnableAll ? -1 : (int) enabledSystems);
    }
}