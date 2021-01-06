using System.Collections.Generic;
using System.Linq;

using VisCPU.Console.Core.Settings;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Subsystems.Origins
{

    public class AddOriginSubSystem : ConsoleSubsystem
    {

        #region Public

        public override void Run( IEnumerable < string > args )
        {
            OriginSettings s = SettingsSystem.GetSettings < OriginSettings >();
            string[] a = args.ToArray();
            string name = a[0];
            string url = a[1];
            s.origins.Add( name, url );
            SettingsSystem.SaveSettings( s );
        }

        #endregion

    }

}
