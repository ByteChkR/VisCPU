using System;
using System.IO;

using VisCPU.Utility;
using VisCPU.Utility.Logging;

namespace VisCPU.HL.Importer
{

    public abstract class AImporter : VisBase, IImporter
    {

        protected override LoggerSystems SubSystem => LoggerSystems.HL_Importer;

        protected string CacheDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");

        protected AImporter()
        {
            Directory.CreateDirectory(CacheDirectory);
        }

        public abstract bool CanImport(string input);

    }

}