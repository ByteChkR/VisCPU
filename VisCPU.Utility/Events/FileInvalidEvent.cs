using VisCPU.Utility.EventSystem;

namespace VisCPU.Utility.Events
{

    public class FileInvalidEvent : ErrorEvent
    {

        #region Public

        public FileInvalidEvent( string file, bool canContinue ) : base(
                                                                        $"The file '{file}' is invalid.",
                                                                        ErrorEventKeys.GENERIC_FILE_INVALID,
                                                                        canContinue
                                                                       )
        {
        }

        #endregion

    }

}
