using System.IO;

using Utility.ExtPP.Base.Interfaces;

namespace Utility.ExtPP.Base
{
    public class FilePathContent : IFileContent
    {

        private readonly string definedName;
        private readonly string filePath;
        private string key;

        public FilePathContent(string filePath, string definedName)
        {
            this.definedName = definedName;
            key = this.filePath = filePath;
        }

        public bool HasValidFilepath => true;

        public bool TryGetLines(out string[] lines)
        {
            lines = null;
            if (!File.Exists(filePath))
            {
                return false;
            }

            lines = File.ReadAllLines(filePath);

            return true;
        }

        public string GetKey()
        {
            return key;
        }

        public void SetKey(string key)
        {
            this.key = key;
        }

        public string GetDefinedName()
        {
            return definedName;
        }

        public string GetFilePath()
        {
            return filePath;
        }

        public override string ToString()
        {
            return key;
        }

    }
}