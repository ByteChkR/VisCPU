using System;

namespace VisCPU.Utility.SharedBase
{

    public readonly struct FileReference : IEquatable < FileReference >
    {

        public string File { get; }

        public object[] LinkerArguments { get; }

        public FileReference( string file, object[] linkerArgs )
        {
            File = file;
            LinkerArguments = linkerArgs;
        }

        public FileReference( string file )
        {
            File = file;
            LinkerArguments = new object[0];
        }

        public bool Equals( FileReference other )
        {
            return File == other.File;
        }

        public override bool Equals( object obj )
        {
            return obj is FileReference other && Equals( other );
        }

        public override int GetHashCode()
        {
            return File != null ? File.GetHashCode() : 0;
        }

    }

}
