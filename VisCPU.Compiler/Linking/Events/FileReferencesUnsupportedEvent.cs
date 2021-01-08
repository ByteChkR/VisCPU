using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Compiler.Linking.Events
{

    public class FileReferencesUnsupportedEvent : ErrorEvent
    {

        #region Public

        public FileReferencesUnsupportedEvent() : base(
                                                       "Single file linker does not support file references.",
                                                       ErrorEventKeys.LINKER_FILE_REFERENCES_UNSUPPORTED,
                                                       false
                                                      )
        {
        }

        #endregion

    }

}
