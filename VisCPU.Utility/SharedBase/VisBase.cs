﻿using VisCPU.Utility.Logging;

namespace VisCPU.Utility.SharedBase
{

    public abstract class VisBase
    {

        protected abstract LoggerSystems SubSystem { get; }

        #region Public

        public virtual void Log( string message )
        {
            Logger.LogMessage( SubSystem, message );
        }

        #endregion

    }

}