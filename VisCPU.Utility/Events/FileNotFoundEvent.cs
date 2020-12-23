namespace VisCPU.Utility.Events
{
    public class FileNotFoundEvent : ErrorEvent
    {
        private const string EVENT_KEY = "file-not-found";

        #region Public

        public FileNotFoundEvent(string file, bool canContinue) : base(
            $"The file '{file}' could not be found.",
            EVENT_KEY,
            canContinue
        )
        {
        }

        #endregion
    }
}