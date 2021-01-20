using System;
using System.IO;
using Newtonsoft.Json;

namespace VisCPU.Utility.Settings.Loader
{

    public class JsonSettingsLoader : SettingsLoader
    {
        #region Public

        public override object LoadSettings( Type t, string file )
        {
            return JsonConvert.DeserializeObject( File.ReadAllText( file ), t );
        }

        public override void SaveSettings( object o, string file )
        {
            File.WriteAllText( file, JsonConvert.SerializeObject( o, Formatting.Indented ) );
        }

        #endregion
    }

}
