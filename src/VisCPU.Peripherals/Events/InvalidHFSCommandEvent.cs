using VisCPU.Peripherals.HostFS;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

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
