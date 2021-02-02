using System;

using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Importer.Events
{

    internal class ImporterTypeInvalidEvent : ErrorEvent
    {

        #region Public

        public ImporterTypeInvalidEvent( Type item ) : base(
                                                            $"Invalid Importer Type: '{item}'",
                                                            ErrorEventKeys.s_ImporterInvalidType,
                                                            false
                                                           )
        {
        }

        #endregion

    }

}
