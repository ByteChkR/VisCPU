using System;
using System.IO;
using VisCPU.Utility;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Importer
{

    public abstract class AImporter : VisBase, IImporter
    {

        protected override LoggerSystems SubSystem => LoggerSystems.HlImporter;

        protected string CacheDirectory => Path.Combine(UnityIsAPieceOfShitHelper.AppRoot, "cache");

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
