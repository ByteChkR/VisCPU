﻿using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Utility.Settings.Events
{

    public class SettingsIODisabledEvent : ErrorEvent
    {

        #region Public

        public SettingsIODisabledEvent( string errMessage ) : base(
                                                                   errMessage,
                                                                   ErrorEventKeys.SETTINGS_IO_DISABLED,
                                                                   false
                                                                  )
        {
        }

        #endregion

    }

}
