using System.Collections.Generic;

namespace VisCPU.Instructions.Emit
{

    public class EmitterResult < T >
    {
        private readonly Emitter < T > m_Emitter;
        private readonly List < T > m_Store = new List < T >();

        #region Public

        public EmitterResult( Emitter < T > emitter )
        {
            m_Emitter = emitter;
        }

        public void Clear()
        {
            m_Store.Clear();
        }

        public void Emit( string instructionKey, params string[] arguments )
        {
            m_Store.Add( m_Emitter.Emit( instructionKey, arguments ) );
        }

        public T[] Get()
        {
            return m_Store.ToArray();
        }

        public void Store( T data )
        {
            m_Store.Add( data );
        }

        #endregion
    }

}
