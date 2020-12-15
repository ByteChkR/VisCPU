using System.Collections.Generic;

using VisCPU.Compiler.Compiler;

namespace VisCPU.Compiler.Linking
{
    public class LinkerTarget
    {

        public readonly IReadOnlyList<object> AdditionalCompilationFlags;
        public readonly FileCompilation FileCompilation;

        public LinkerTarget(FileCompilation fileCompilation, object[] inFileCompilationFlags)
        {
            FileCompilation = fileCompilation;
            AdditionalCompilationFlags = inFileCompilationFlags;
        }

    }
}