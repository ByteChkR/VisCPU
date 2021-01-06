﻿using VisCPU.Peripherals.HostFS;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Peripherals.Events
{

    public class InvalidHFSCommandEvent : ErrorEvent
    {

        #region Public

        public InvalidHFSCommandEvent( HostFileSystemCommands command ) : base(
                                                                               $"Invalid Command: {command}",
                                                                               ErrorEventKeys.HFS_INVALID_COMMAND,
                                                                               false
                                                                              )
        {
        }

        #endregion

    }

}
