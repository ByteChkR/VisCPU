using System.Collections.Generic;
using System.Linq;

using VisCPU.Compiler.Assembler;
using VisCPU.Compiler.Linking;
using VisCPU.Utility;

namespace VisCPU.Compiler.Compiler
{
    public class Compilation
    {
        private readonly AssemblyGenerator assemblyGenerator;

        private readonly Linker linker;

        public List<byte> ByteCode { get; private set; }

        public LinkerInfo LinkerInfo { get; private set; }

        public LinkerResult LinkerResult { get; private set; }

        #region Public

        public Compilation(Linker linker, AssemblyGenerator assemblyGenerator)
        {
            this.linker = linker;
            this.assemblyGenerator = assemblyGenerator;
        }

        public void Compile(string file)
        {
            FileCompilation fc = new FileCompilation(new FileReference(file));

            LinkerTarget linkTarget = new LinkerTarget(fc, fc.Reference.LinkerArguments);

            LinkerResult linkResult = linker.Link(linkTarget, this);

            LinkerInfo = CreateFromResult(linkResult);
            LinkerResult = linkResult;

            ByteCode = assemblyGenerator.Assemble(linkResult);
        }
        public static LinkerInfo CreateFromResult(LinkerResult result)
        {
            return new LinkerInfo
                   {
                       Constants = result.Constants,
                       DataSectionHeader = result.DataSectionHeader,
                       Labels = result.Labels,
                       Source = result.LinkedBinary.FirstOrDefault()?.FirstOrDefault()?.OriginalText
                   };
        }

        #endregion
    }
}