using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Compiler.Assembler.Events
{

    public class InvalidArgumentCountEvent : ErrorEvent
    {

        #region Public

        public InvalidArgumentCountEvent(string instr, int argCount) : base(
                                                                            $"Too many arguments for Instruction '{instr}' argument count: {argCount}",
                                                                            ErrorEventKeys.ASM_GEN_TOO_MANY_ARGS,
                                                                            false
                                                                           )
        {
        }

        #endregion

    }

    public class InvalidInstructionArgumentCountEvent : ErrorEvent
    {

        #region Public

        
        public InvalidInstructionArgumentCountEvent(string instr, int argCount) : base(
                                                                            $"The Instruction '{instr}' can not be used as it has {argCount} arguments while the max argument count is 3",
                                                                            ErrorEventKeys.ASM_GEN_TOO_MANY_ARGS,
                                                                            false
                                                                           )
        {
        }

        #endregion

    }

}
