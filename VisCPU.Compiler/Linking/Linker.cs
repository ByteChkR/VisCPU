
using VisCPU.Compiler.Compiler;
using VisCPU.Utility.Events;
using VisCPU.Utility.Logging;

namespace VisCPU.Compiler.Linking
{

    public abstract class Linker : VisBase
    {

        protected override LoggerSystems SubSystem => LoggerSystems.Linker;

        public abstract LinkerResult Link(LinkerTarget target, Compilation compilation);

    }
}