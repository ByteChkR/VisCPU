using VisCPU.Utility.Logging;

namespace VisCPU.Utility.Events
{

    public abstract class VisBase
    {

        protected abstract LoggerSystems SubSystem { get; }

        public virtual void Log( string message )
        {
            Logger.LogMessage( SubSystem, message );
        }
        
    }

}