using VisCPU.Peripherals.HostFS;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Peripherals.Events
{

    internal class InvalidHfsCommandEvent : ErrorEvent
    {
        #region Public

        public InvalidHfsCommandEvent( HostFileSystemCommands command ) : base(
            $"Invalid Command: {command}",
            ErrorEventKeys.s_HfsInvalidCommand,
            false
        )
        {
        }

        #endregion
    }

}
