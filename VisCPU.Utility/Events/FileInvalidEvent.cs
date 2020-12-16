namespace VisCPU.Utility.Events
{

    public class FileInvalidEvent : ErrorEvent
    {

        private const string EVENT_KEY = "file-invalid";

        #region Public

        public FileInvalidEvent( string file, bool canContinue ) : base(
                                                                        $"The file '{file}' is invalid.",
                                                                        EVENT_KEY,
                                                                        canContinue
                                                                       )
        {
        }

        #endregion

    }

}
