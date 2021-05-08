﻿using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Compiler.Events
{

    public class StaticParseFailedEvent : ErrorEvent
    {
        #region Public

        public StaticParseFailedEvent( string data ) : base(
            $"Can not parse data '{data}'",
            ErrorEventKeys.s_HlStaticParseFailed,
            false
        )
        {
        }

        #endregion
    }

}
