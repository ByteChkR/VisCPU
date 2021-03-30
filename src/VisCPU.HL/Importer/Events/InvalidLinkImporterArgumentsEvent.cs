using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Importer.Events
{

    internal class InvalidLinkImporterArgumentsEvent : ErrorEvent
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
