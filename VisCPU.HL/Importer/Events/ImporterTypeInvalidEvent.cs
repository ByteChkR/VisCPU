using System;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

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
