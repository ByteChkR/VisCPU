using System;
using System.IO;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core
{

    public class CLISettings
    {

        [Argument( Name = "continuous" )]
        [Argument( Name = "loop" )]
        public bool Continuous = false;

        [Argument( Name = "waitOnExit" )]
        public bool WaitOnExit = false;

        #region Public

        public static CLISettings Create()
        {
            return Settings.GetSettings < CLISettings >();
        }

        #endregion

        #region Private

        static CLISettings()
        {
            Settings.RegisterDefaultLoader(
                                           new JSONSettingsLoader(),
                                           Path.Combine(
                                                        AppDomain.CurrentDomain.BaseDirectory,
                                                        "config/console.json"
                                                       ),
                                           new CLISettings()
                                          );
        }

        #endregion

    }

}
