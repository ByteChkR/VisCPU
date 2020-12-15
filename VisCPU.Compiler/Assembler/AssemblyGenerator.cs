using System.Collections.Generic;

using VisCPU.Compiler.Linking;
using VisCPU.Utility.Events;
using VisCPU.Utility.Logging;

namespace VisCPU.Compiler.Assembler
{
    public abstract class AssemblyGenerator : VisBase
    {

        protected override LoggerSystems SubSystem => LoggerSystems.AssemblyGenerator;

        public abstract List<byte> Assemble(LinkerResult result);

    }
}