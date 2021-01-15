using System.Collections.Generic;

using VisCPU.HL.Modules.Resolvers;

namespace VisCPU.Console.Core.Subsystems.Origins
{

    public class ListOriginSubSystem : ConsoleSubsystem
    {

        #region Public

        public override void Help()
        {
        }

        public override void Run( IEnumerable < string > args )
        {
            ModuleResolverSettings s = ModuleResolverSettings.Create();

            foreach ( KeyValuePair < string, string > keyValuePair in s.ModuleOrigins )
            {
                Log( $"{keyValuePair.Key} : {keyValuePair.Value}" );
            }
        }

        #endregion

    }

}
