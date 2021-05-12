using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Peripherals.Events
{

    public class HostFileSystemReadFailureEvent : ErrorEvent
    {

        #region Public

        public HostFileSystemReadFailureEvent( string errMessage ) : base(
                                                                          errMessage,
                                                                          ErrorEventKeys.s_HfsReadFailure,
                                                                          true
                                                                         )
        {
        }

        #endregion

    }

}
