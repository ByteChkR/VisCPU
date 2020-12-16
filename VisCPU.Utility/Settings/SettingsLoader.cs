using System;

namespace VisCPU.Utility.Settings
{

    public abstract class SettingsLoader
    {

        #region Public

        public abstract object LoadSettings( Type t, string file );

        public abstract void SaveSettings( object o, string file );

        #endregion

    }

}
