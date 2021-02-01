using VisCPU.Utility.ArgumentParser;

namespace VisCPU.Utility.EventSystem
{

    public class EventManagerSettings
    {
        [Argument( Name = "event-system:interactive" )]
        public bool Interactive;

        [field: Argument( Name = "event-system:ignore" )]
        public string[] IgnoredEvents { get; set; } = new string[0];
    }

}
