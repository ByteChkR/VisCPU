﻿using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.TypeSystem.Events
{

    public class HLMemberNotFoundEvent : ErrorEvent
    {

        #region Public

        public HLMemberNotFoundEvent( string name ) : base(
                                                           $"Can not Find Member: {name}",
                                                           ErrorEventKeys.HL_MEMBER_NOT_FOUND,
                                                           false
                                                          )
        {
        }

        #endregion

    }

}
