using System.Collections.Generic;

using VisCPU.Compiler.Assembler;
using VisCPU.Compiler.Linking;
using VisCPU.Utility;

namespace VisCPU.Compiler.Compiler
{
    public class Compilation
    {

        private readonly AssemblyGenerator assemblyGenerator;

        private readonly Linker linker;

        public Compilation(Linker linker, AssemblyGenerator assemblyGenerator)
        {
            this.linker = linker;
            this.assemblyGenerator = assemblyGenerator;
        }

        public List<byte> ByteCode { get; private set; }

        public LinkerInfo LinkerInfo { get; private set; }

        public LinkerResult LinkerResult { get; private set; }

        public void Compile(string file)
        {
            FileCompilation fc = new FileCompilation(new FileReference(file));

            LinkerTarget linkTarget = new LinkerTarget(fc, fc.Reference.LinkerArguments);

            LinkerResult linkResult = linker.Link(linkTarget, this);

            LinkerInfo = LinkerInfo.CreateFromResult(linkResult);
            LinkerResult = linkResult;

            ByteCode = assemblyGenerator.Assemble(linkResult);
        }

    }
}