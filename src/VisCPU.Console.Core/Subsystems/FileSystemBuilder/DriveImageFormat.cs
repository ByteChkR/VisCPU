using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Console.Core.Subsystems.FileSystemBuilder
{

    public abstract class DriveImageFormat : VisBase
    {
        public abstract string FormatName { get; }

        public abstract string[] SupportedExtensions { get; }

        public abstract bool SupportsDirectoryInput { get; }

        protected override LoggerSystems SubSystem => LoggerSystems.DriveImageBuilder;

        #region Public

        public abstract void Pack( string input );

        public abstract void Unpack( string input );

        public virtual object[] GetSettingsObjects()
        {
            return new object[0];
        }

        #endregion
    }

}