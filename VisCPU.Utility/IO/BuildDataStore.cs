using System.IO;
using System.Linq;

namespace VisCPU.Utility.IO
{

    public class BuildDataStore
    {

        private string RootDir;
        private IBuildDataStoreType[] Types;

        #region Public

        public BuildDataStore( string rootDir, params IBuildDataStoreType[] types )
        {
            Types = types;
            RootDir = Directory.CreateDirectory( Path.Combine( rootDir, "build" ) ).FullName;

            foreach ( IBuildDataStoreType buildDataStoreType in Types )
            {
                buildDataStoreType.Initialize( RootDir );
            }
        }

        public string GetStorePath( string storeType, string file )
        {
            return Types.First( x => x.TypeName == storeType ).GetStoreDirectory( RootDir, file );
        }

        #endregion

    }

}
