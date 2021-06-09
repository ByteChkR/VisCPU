using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.IO.Settings.Loader;

namespace VisCPU.Console.Core.Settings
{

    public class CliSettings
    {

        [field: Argument( Name = "cli:loop" )]
        public bool Continuous { get; set; }

        [field: Argument( Name = "cli:waitOnExit" )]
        public bool WaitOnExit { get; set; }

        [field: Argument(Name = "cli:configs")]
        public string[] Configs { get; set; } = new[] { "./default.args" };
        [field: Argument(Name = "cli:logfile")]
        public string LogFile { get; set; } =null;

        #region Private

        static CliSettings()
        {
            SettingsCategory cliCategory = SettingsCategories.Get( "sdk", true );

            SettingsManager.RegisterDefaultLoader(
                                                  new JsonSettingsLoader(),
                                                  cliCategory,
                                                  "cli.json",
                                                  new CliSettings()
                                                 );
        }

        #endregion

    }

}
