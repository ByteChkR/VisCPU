using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Importer.Events
{

    internal class ImporterNotFoundEvent : ErrorEvent
    {
        #region Public

        public ImporterNotFoundEvent( string item ) : base(
            $"Can not Find Importer for Item: '{item}'",
            ErrorEventKeys.s_ImporterNotFound,
            false
        )
        {
        }

        #endregion
    }

}
