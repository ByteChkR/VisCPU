using System.IO;

using VisCPU.Utility;
using VisCPU.Utility.Logging;

namespace VisCPU
{

    public abstract class Peripheral : VisBase
    {

        protected override LoggerSystems SubSystem => LoggerSystems.Peripherals;

        #region Unity Event Functions

        public virtual void Reset()
        {
        }

        #endregion

        #region Public

        public abstract bool CanRead( uint address );

        public abstract bool CanWrite( uint address );

        public abstract uint ReadData( uint address );

        public abstract void WriteData( uint address, uint data );

        public virtual void Dump( Stream str )
        {
        }

        public override void Log( string message )
        {
            base.Log( $"[{GetType().Name}]" + message );
        }

        #endregion

    }

}
