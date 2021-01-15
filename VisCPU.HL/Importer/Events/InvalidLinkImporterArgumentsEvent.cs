using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Importer.Events
{

    public class InvalidLinkImporterArgumentsEvent : ErrorEvent
    {

        #region Public

        public InvalidLinkImporterArgumentsEvent( string args ) : base(
                                                                       $"Invalid Arguments: {args}",
                                                                       ErrorEventKeys.s_LinkerImporterInvalidArguments,
                                                                       false
                                                                      )
        {
        }

        #endregion

    }

}
