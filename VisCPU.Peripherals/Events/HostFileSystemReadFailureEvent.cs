using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Peripherals.Events
{

    public class HostFileSystemReadFailureEvent : ErrorEvent
    {
        #region Public

        public HostFileSystemReadFailureEvent( string errMessage ) : base(
            errMessage,
            ErrorEventKeys.s_HfsReadFailure,
            true )
        {
        }

        #endregion
    }

}