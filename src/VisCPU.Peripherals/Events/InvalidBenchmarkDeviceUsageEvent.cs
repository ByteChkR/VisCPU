using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

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
