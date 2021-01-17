using VisCPU.Peripherals.HostFS;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Peripherals.Events
{

    internal class InvalidHFSCommandEvent : ErrorEvent
    {

        #region Public

        public InvalidHFSCommandEvent( HostFileSystemCommands command ) : base(
                                                                               $"Invalid Command: {command}",
                                                                               ErrorEventKeys.s_HfsInvalidCommand,
                                                                               false
                                                                              )
        {
        }

        #endregion

    }

}
