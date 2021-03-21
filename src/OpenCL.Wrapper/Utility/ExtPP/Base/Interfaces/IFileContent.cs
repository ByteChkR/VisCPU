namespace Utility.ExtPP.Base.Interfaces
{
    public interface IFileContent
    {

        bool HasValidFilepath { get; }

        bool TryGetLines(out string[] lines);

        string GetKey();

        void SetKey(string key);

        string GetFilePath();

        string GetDefinedName();

    }
}