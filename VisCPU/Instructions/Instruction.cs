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

        #endregion

    }

}
