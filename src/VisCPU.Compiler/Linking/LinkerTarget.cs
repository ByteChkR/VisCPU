using System.Collections.Generic;

using VisCPU.Compiler.Compiler;

namespace VisCPU.Compiler.Linking
{

    public class LinkerTarget
    {

        public IReadOnlyList < object > AdditionalCompilationFlags { get; }

        public FileCompilation FileCompilation { get; }

        #region Public

        public LinkerTarget( FileCompilation fileCompilation, object[] inFileCompilationFlags )
        {
            FileCompilation = fileCompilation;
            AdditionalCompilationFlags = inFileCompilationFlags;
        }

        #endregion

    }

}
