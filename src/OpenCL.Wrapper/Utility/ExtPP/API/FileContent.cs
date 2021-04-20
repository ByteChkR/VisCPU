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

        public bool HasValidFilepath => false;

        private string Key => incDir + "/memoryFile" + extension;

        private string Path => incDir + "/memoryFile" + extension;

        #region Public

        public FileContent( string[] lines, string incDir, string ext )
        {
            this.lines = lines;
            this.incDir = incDir;
            extension = ext;
        }

        public string GetDefinedName()
        {
            return Key;
        }

        public string GetFilePath()
        {
            return Path;
        }

        public string GetKey()
        {
            return Key;
        }

        public void SetKey( string key )
        {
            //Nothing
        }

        public override string ToString()
        {
            return Key;
        }

        public bool TryGetLines( out string[] lines )
        {
            lines = this.lines;

            return true;
        }

        #endregion

    }

}
