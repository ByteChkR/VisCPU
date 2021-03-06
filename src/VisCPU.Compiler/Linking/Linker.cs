﻿using VisCPU.Compiler.Compiler;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Compiler.Linking
{

    public abstract class Linker : VisBase
    {

        protected override LoggerSystems SubSystem => LoggerSystems.Linker;

        #region Public

        public abstract LinkerResult Link( LinkerTarget target, Compilation compilation );

        #endregion

    }

}
