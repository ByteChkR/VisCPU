using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.Settings.Loader;

namespace VisCPU.Console.Core.Subsystems.FileSystemBuilder
{

    public class DriveImageFormatV1Settings
    {

        [Argument( Name = "dib:image.v1.path.prefix" )]
        public string FileSystemPrefix = "";

        [Argument( Name = "dib:image.v1.size" )]
        public uint DiskSize = 0;

        [Argument( Name = "dib:image.v1.unpack" )]
        public bool UnpackData = true;

        [Argument( Name = "dib:image.v1.unpack.ignore" )]
        public string[] UnpackIgnoreExtensions = new[] { "vbin" };

        #region Private

        static DriveImageFormatV1Settings()
        {
            SettingsCategory diCategory = SettingsCategories.Get( "sdk.utils.disk.formats.v1", true );

            SettingsManager.RegisterDefaultLoader(
                                                  new JsonSettingsLoader(),
                                                  diCategory,
                                                  "v1-format.json",
                                                  new DriveImageFormatV1Settings()
                                                 );
        }

        #endregion

    }

}
