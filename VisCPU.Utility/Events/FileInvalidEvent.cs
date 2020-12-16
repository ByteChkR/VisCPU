namespace VisCPU.Utility.Events
{

    public class FileInvalidEvent : ErrorEvent
    {

        private const string EVENT_KEY = "file-invalid";
        public FileInvalidEvent(string file, bool canContinue) : base($"The file '{file}' is invalid.", EVENT_KEY, canContinue)
        {
        }

    }

}