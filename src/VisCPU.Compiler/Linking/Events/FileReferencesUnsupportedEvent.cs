using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.Compiler.Linking.Events
{

    internal class FileReferencesUnsupportedEvent : ErrorEvent
    {

        #region Public

        public FileReferencesUnsupportedEvent() : base(
                                                       "Single file linker does not support file references.",
                                                       ErrorEventKeys.s_LinkerFileReferencesUnsupported,
                                                       false
                                                      )
        {
        }

        #endregion

    }

}
