using System.IO;
using System.Linq;

namespace VisCPU.Utility.IO
{

    public class BuildDataStore
    {

        private string m_RootDir;
        private IBuildDataStoreType[] m_Types;

        #region Public

        public BuildDataStore( string rootDir, params IBuildDataStoreType[] types )
        {
            m_Types = types;
            m_RootDir = Directory.CreateDirectory( Path.Combine( rootDir, "build" ) ).FullName;

            foreach ( IBuildDataStoreType buildDataStoreType in m_Types )
            {
                buildDataStoreType.Initialize( m_RootDir );
            }
        }

        public string GetStorePath( string storeType, string file )
        {
            return m_Types.First( x => x.TypeName == storeType ).GetStoreDirectory( m_RootDir, file );
        }

        #endregion

    }

}
