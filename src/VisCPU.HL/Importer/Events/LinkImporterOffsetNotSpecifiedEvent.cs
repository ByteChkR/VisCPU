﻿using System.IO;

using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.Importer.Events
{

    internal class LinkImporterOffsetNotSpecifiedEvent : ErrorEvent
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
