using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Events
{

    public class InstructionNotFoundEvent : ErrorEvent
    {

        #region Public

        public InstructionNotFoundEvent( byte opCode ) : base(
                                                              $"Can not find Instruction with op code: {opCode}",
                                                              ErrorEventKeys.INSTR_OP_NOT_FOUND,
                                                              false
                                                             )
        {
        }

        public InstructionNotFoundEvent( Instruction instruction ) : base(
                                                                          $"Can not find Instruction {instruction.Key}",
                                                                          ErrorEventKeys.INSTR_OP_NOT_FOUND,
                                                                          false
                                                                         )
        {
        }

        public InstructionNotFoundEvent( string key, int args ) : base(
                                                                       $"Can not find Instruction {key} with argument count {args}",
                                                                       ErrorEventKeys.INSTR_OP_NOT_FOUND,
                                                                       false
                                                                      )
        {
        }

        #endregion

    }

}
