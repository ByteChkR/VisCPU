﻿using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Events
{

    public class DuplicateVarDefinitionEvent : ErrorEvent
    {

        #region Public

        public DuplicateVarDefinitionEvent( string varName ) : base(
                                                                    $"Duplicate Definition of: {varName}",
                                                                    ErrorEventKeys.s_HlVarDuplicateDef,
                                                                    false
                                                                   )
        {
        }

        #endregion

    }

}
