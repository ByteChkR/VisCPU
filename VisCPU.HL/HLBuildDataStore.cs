using System.IO;
using VisCPU.Utility.IO;

namespace VisCPU.HL
{

    public class HLBuildDataStore : IBuildDataStoreType
    {
        public string TypeName => "HL2VASM";

        #region Public

        public string GetStoreDirectory( string rootDir, string file )
        {
            return Path.Combine(
                Directory.CreateDirectory( Path.Combine( rootDir, TypeName ) ).FullName,
                $"{( uint ) Path.GetDirectoryName( file ).GetHashCode()}_{Path.GetFileName( file )}.vasm"
            );
        }

        public void Initialize( string rootDir )
        {
            if ( Directory.Exists( Path.Combine( rootDir, TypeName ) ) )
            {
                Directory.Delete( Path.Combine( rootDir, TypeName ), true );
            }

            Directory.CreateDirectory( Path.Combine( rootDir, TypeName ) );
        }

        #endregion
    }

}
