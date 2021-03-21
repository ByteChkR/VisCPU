using Utility.ExtPP.Base.Interfaces;

namespace Utility.ExtPP.API
{
    /// <summary>
    ///     File Content that is used as an abstraction to files
    /// </summary>
    public class FileContent : IFileContent
    {

        private readonly string extension;
        private readonly string incDir;
        private readonly string[] lines;

        public FileContent(string[] lines, string incDir, string ext)
        {
            this.lines = lines;
            this.incDir = incDir;
            extension = ext;
        }

        private string Key => incDir + "/memoryFile" + extension;

        private string Path => incDir + "/memoryFile" + extension;

        public bool HasValidFilepath => false;

        public bool TryGetLines(out string[] lines)
        {
            lines = this.lines;
            return true;
        }

        public string GetKey()
        {
            return Key;
        }

        public void SetKey(string key)
        {
            //Nothing
        }

        public string GetFilePath()
        {
            return Path;
        }

        public string GetDefinedName()
        {
            return Key;
        }

        public override string ToString()
        {
            return Key;
        }

    }
}