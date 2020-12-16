using VisCPU.Utility.Events;

namespace VisCPU.Compiler.Linking.Events
{

    public class FileReferencesUnsupportedEvent : ErrorEvent
    {

        private const string EVENT_KEY = "lnk-file-ref-unsupported";

        #region Public

        public FileReferencesUnsupportedEvent() : base(
                                                       "Single file linker does not support file references.",
                                                       EVENT_KEY,
                                                       false
                                                      )
        {
        }

        #endregion

    }

}
