using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Importer.Events
{

    public class ImporterNotFoundEvent : ErrorEvent
    {

        #region Public

        public ImporterNotFoundEvent( string item ) : base(
                                                           $"Can not Find Importer for Item: '{item}'",
                                                           ErrorEventKeys.IMPORTER_NOT_FOUND,
                                                           false
                                                          )
        {
        }

        #endregion

    }

}
