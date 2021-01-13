using System.Collections.Generic;
using System.Linq;

using VisCPU.HL.Modules.Resolvers;
using VisCPU.Utility.Settings;

namespace VisCPU.Console.Core.Subsystems.Origins
{

    public class RemoveOriginSubSystem : ConsoleSubsystem
    {

        #region Public

        public override void Run( IEnumerable < string > args )
        {
            ModuleResolverSettings s = ModuleResolverSettings.Create();
            string name = args.First();
            s.ModuleOrigins.Remove( name );
            SettingsSystem.SaveSettings( s );
        }

        #endregion

    }

}
