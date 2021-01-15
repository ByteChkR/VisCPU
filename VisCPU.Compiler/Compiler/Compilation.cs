using System.Collections.Generic;
using System.Linq;

using VisCPU.Compiler.Assembler;
using VisCPU.Compiler.Linking;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Compiler.Compiler
{

    public class Compilation
    {

        private readonly AssemblyGenerator m_AssemblyGenerator;

        private readonly Linker m_Linker;

        public List < byte > ByteCode { get; private set; }

        public LinkerInfo LinkerInfo { get; private set; }

        public LinkerResult LinkerResult { get; private set; }

        #region Public

        public Compilation( Linker linker, AssemblyGenerator assemblyGenerator )
        {
            m_Linker = linker;
            m_AssemblyGenerator = assemblyGenerator;
        }

        public static LinkerInfo CreateFromResult( LinkerResult result )
        {
            return new LinkerInfo
                   {
                       Constants = result.Constants,
                       DataSectionHeader = result.DataSectionHeader,
                       Labels = result.Labels,
                       Source = result.LinkedBinary.FirstOrDefault()?.FirstOrDefault()?.OriginalText
                   };
        }

        public void Compile( string file )
        {
            FileCompilation fc = new FileCompilation( new FileReference( file ) );

            LinkerTarget linkTarget = new LinkerTarget( fc, fc.Reference.LinkerArguments );

            LinkerResult linkResult = m_Linker.Link( linkTarget, this );

            LinkerInfo = CreateFromResult( linkResult );
            LinkerResult = linkResult;

            ByteCode = m_AssemblyGenerator.Assemble( linkResult );
        }

        #endregion

    }

}
