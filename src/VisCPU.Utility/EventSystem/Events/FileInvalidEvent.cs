namespace VisCPU.Utility.EventSystem.Events
{

    public class FileInvalidEvent : ErrorEvent
    {
        #region Public

        public FileInvalidEvent( string file, bool canContinue ) : base(
            $"The file '{file}' is invalid.",
            ErrorEventKeys.s_GenericFileInvalid,
            canContinue
        )
        {
        }

        #endregion
    }

}
