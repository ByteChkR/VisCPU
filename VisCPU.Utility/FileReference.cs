namespace VisCPU.Utility
{
    public readonly struct FileReference
    {
        public readonly string File;
        public readonly object[] LinkerArguments;

        public FileReference(string file, object[] linkerArgs)
        {
            File = file;
            LinkerArguments = linkerArgs;
        }

        public FileReference(string file)
        {
            File = file;
            LinkerArguments = new object[0];
        }
    }
}