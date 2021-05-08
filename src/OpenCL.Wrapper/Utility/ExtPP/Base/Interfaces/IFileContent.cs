namespace Utility.ExtPP.Base.Interfaces
{

    public interface IFileContent
    {
        string GetDefinedName();

        string GetFilePath();

        string GetKey();

        bool HasValidFilepath { get; }

        void SetKey( string key );

        bool TryGetLines( out string[] lines );
    }

}
