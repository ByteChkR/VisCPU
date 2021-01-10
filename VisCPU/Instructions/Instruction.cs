using System;

using VisCPU.Utility.SharedBase;

namespace VisCPU.Instructions
{

    public abstract class Instruction : VisBase
    {

        public abstract uint Cycles { get; }

        public abstract string Key { get; }

        public abstract uint InstructionSize { get; }

        public abstract uint ArgumentCount { get; }

        #region Public

        public abstract void Process( CPU cpu );

        public override void Log( string message )
        {
            base.Log( $"[{Key}] " + message );
        }

        #endregion

        #region Protected

        protected void Log( CPU cpu, string mesg )
        {
            Log( $"[0x{Convert.ToString( cpu.ProgramCounter, 16 )}]: {mesg}" );
        }

        #endregion

    }

}
