using VisCPU.Utility.EventSystem;

namespace VisCPU.Utility.Events
{

    public class FileNotFoundEvent : ErrorEvent
    {

        #region Public

        public FileNotFoundEvent( string file, bool canContinue ) : base(
                                                                         $"The file '{file}' could not be found.",
                                                                         ErrorEventKeys.s_GenericFileNotFound,
                                                                         canContinue
                                                                        )
        {
        }

        public override string ToString()
        {
            return Message;
        }

        #endregion

    }

}
