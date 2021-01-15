using System.IO;

using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Importer.Events
{

    public class LinkImporterOffsetNotSpecifiedEvent : ErrorEvent
    {

        #region Public

        public LinkImporterOffsetNotSpecifiedEvent( string file ) : base(
                                                                         $"Offset was not specified and file '{file}({Path.GetFullPath( file )})' does not exist.",
                                                                         ErrorEventKeys.s_LinkerImporterNoOffset,
                                                                         false
                                                                        )
        {
        }

        #endregion

    }

}
