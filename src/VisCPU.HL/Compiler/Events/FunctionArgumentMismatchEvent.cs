﻿using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Compiler.Events
{

    public class FunctionArgumentMismatchEvent : ErrorEvent
    {

        #region Public

        public FunctionArgumentMismatchEvent( string errMessage ) : base(
                                                                         errMessage,
                                                                         ErrorEventKeys.s_HlFunctionArgumentMismatch,
                                                                         false
                                                                        )
        {
        }

        #endregion

    }

}
