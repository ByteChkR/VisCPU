using System.Collections.Generic;

using VisCPU.Compiler.Linking;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Compiler.Assembler
{

    public abstract class AssemblyGenerator : VisBase
    {

        protected override LoggerSystems SubSystem => LoggerSystems.AssemblyGenerator;

        #region Public

        public abstract List < byte > Assemble( LinkerResult result );

        #endregion

    }

}
