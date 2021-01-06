using System;

using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Importer.Events
{

    public class ImporterTypeInvalidEvent : ErrorEvent
    {

        #region Public

        public ImporterTypeInvalidEvent( Type item ) : base(
                                                            $"Invalid Importer Type: '{item}'",
                                                            ErrorEventKeys.IMPORTER_INVALID_TYPE,
                                                            false
                                                           )
        {
        }

        #endregion

    }

}
