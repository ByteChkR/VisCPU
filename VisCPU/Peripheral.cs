using System.IO;

using VisCPU.Utility.Events;
using VisCPU.Utility.Logging;

namespace VisCPU
{
    public abstract class Peripheral : VisBase
    {

        protected override LoggerSystems SubSystem => LoggerSystems.Peripherals;

        public abstract bool CanWrite(uint address);

        public abstract bool CanRead(uint address);

        public abstract void WriteData(uint address, uint data);

        public abstract uint ReadData(uint address);

        public virtual void Reset()
        {
        }

        public virtual void Dump(Stream str)
        {
        }

        public override void Log( string message )
        {
            base.Log( $"[{GetType().Name}]"+ message );
        }

    }
}