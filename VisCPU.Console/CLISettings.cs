using VisCPU.Utility.Settings;

namespace VisCPU.Console
{

    public class CLISettings
    {

        public static CLISettings Create() => Settings.GetSettings < CLISettings >();

        static CLISettings()
        {

            Settings.RegisterDefaultLoader(
                                           new JSONSettingsLoader(),
                                           "config/console.json",
                                           new CLISettings()
                                          );
        }
        
        [Argument(Name = "continuous")]
        [Argument(Name = "loop")]
        public bool Continuous = false;
        [Argument(Name = "waitOnExit")]
        public bool WaitOnExit = false;

    }

}