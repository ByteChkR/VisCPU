using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Importer.Events
{

    public class InvalidLinkImporterArgumentsEvent : ErrorEvent
    {

        #region Public

        public InvalidLinkImporterArgumentsEvent( string args ) : base(
                                                                       $"Invalid Arguments: {args}",
                                                                       ErrorEventKeys.LINKER_IMPORTER_INVALID_ARGUMENTS,
                                                                       false
                                                                      )
        {
        }

        #endregion

    }

}
