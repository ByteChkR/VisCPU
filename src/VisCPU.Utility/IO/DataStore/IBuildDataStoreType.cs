namespace VisCPU.Utility.IO.DataStore
{

    public interface IBuildDataStoreType
    {

        string GetStoreDirectory( string rootDir, string file );

        void Initialize( string rootDir );

        string TypeName { get; }

    }

}
