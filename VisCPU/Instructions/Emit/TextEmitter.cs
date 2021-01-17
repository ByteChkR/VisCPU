using System.Text;

using VisCPU.Instructions.Emit.Events;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Instructions.Emit
{

    public class TextEmitter : Emitter < string >
    {

        #region Public

        public override string Emit( string instructionKey, params string[] arguments )
        {
            StringBuilder sb = new StringBuilder( instructionKey );

            Instruction instr = CPUSettings.InstructionSet.GetInstruction(
                                                                          instructionKey,
                                                                          arguments.Length
                                                                         );

            if ( arguments.Length > instr.ArgumentCount )
            {
                EventManager < ErrorEvent >.SendEvent(
                                                      new InvalidArgumentCountEvent( instructionKey, arguments.Length )
                                                     );
            }

            foreach ( string aToken in arguments )
            {
                sb.Append( $" {aToken}" );
            }

            return sb.ToString();
        }

        #endregion

    }

}
