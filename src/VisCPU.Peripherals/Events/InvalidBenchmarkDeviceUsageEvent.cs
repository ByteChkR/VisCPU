using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Peripherals.Events
{

    internal class InvalidBenchmarkDeviceUsageEvent : ErrorEvent
    {
        #region Public

        public InvalidBenchmarkDeviceUsageEvent( string message ) : base(
            message,
            ErrorEventKeys.s_BenchmarkDeviceInvalidUsage,
            false
        )
        {
        }

        #endregion
    }

}
