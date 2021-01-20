using System;
using System.IO;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Importer
{

    public abstract class AImporter : VisBase, IImporter
    {
        public static string CacheRoot { get; set; } = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "cache" );

        protected override LoggerSystems SubSystem => LoggerSystems.HlImporter;

        protected string CacheDirectory => CacheRoot;

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
