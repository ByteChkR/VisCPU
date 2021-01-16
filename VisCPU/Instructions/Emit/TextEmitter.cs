using System.Collections.Generic;
using System.Text;

using VisCPU.Instructions.Emit.Events;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Instructions.Emit
{

    public class EmitterResult < T >
    {

        private readonly Emitter < T > m_Emitter;
        private readonly List < T > store = new List < T >();

        #region Public

        public EmitterResult( Emitter < T > emitter )
        {
            m_Emitter = emitter;
        }

        public void Clear()
        {
            store.Clear();
        }

        public void Emit( string instructionKey, params string[] arguments )
        {
            store.Add( m_Emitter.Emit( instructionKey, arguments ) );
        }

        public T[] Get()
        {
            return store.ToArray();
        }

        public void Store( T data )
        {
            store.Add( data );
        }

        #endregion

    }

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
