using System;
using System.IO;

using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Importer
{

    public abstract class AImporter : VisBase, IImporter
    {

        protected override LoggerSystems SubSystem => LoggerSystems.HL_Importer;

        protected string CacheDirectory => Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "cache" );

        #region Public

        public abstract bool CanImport( string input );

        #endregion

        #region Protected

        protected AImporter()
        {
            Directory.CreateDirectory( CacheDirectory );
        }

        #endregion

    }

}
