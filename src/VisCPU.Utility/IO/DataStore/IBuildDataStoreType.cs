namespace VisCPU.Utility.IO
{

    public interface IBuildDataStoreType
    {
        string GetStoreDirectory( string rootDir, string file );

        void Initialize( string rootDir );

        string TypeName { get; }
    }

}
