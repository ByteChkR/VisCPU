﻿using VisCPU.HL.Parser;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.TypeSystem.Events
{

    internal abstract class InvalidHLMemberModifiersEvent : ErrorEvent
    {

        #region Protected

        protected InvalidHLMemberModifiersEvent( HLTokenType a, HLTokenType b ) : base(
             $"Token '{a}' can not be used together with '{b}'",
             ErrorEventKeys.s_HlInvalidMemberModifiers,
             false
            )
        {
        }

        #endregion

    }

}
