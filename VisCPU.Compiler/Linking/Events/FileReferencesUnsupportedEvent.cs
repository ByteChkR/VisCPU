using VisCPU.Utility.Events;

namespace VisCPU.Compiler.Linking
{

    public class FileReferencesUnsupportedEvent:ErrorEvent
    {

        private const string EVENT_KEY = "lnk-file-ref-unsupported";
        public FileReferencesUnsupportedEvent( ) : base("Single file linker does not support file references.", EVENT_KEY, false )
        {
        }

    }

}