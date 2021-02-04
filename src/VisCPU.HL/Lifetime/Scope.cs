using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VisCPU.HL.Lifetime
{

    public class Scope < V > : IEnumerable < KeyValuePair < string, V > >
    {

        private readonly string m_Prefix;
        private readonly Scope < V > m_Parent = null;
        private readonly Dictionary < string, V > m_Data = new Dictionary < string, V >();

        #region Public

        public Scope() : this( "_" )
        {
        }

        public Scope( string prefix )
        {
            m_Prefix = prefix;
        }

        public Scope( Scope < V > parent, string prefix ) : this( prefix )
        {
            m_Parent = parent;
        }
        
        public bool Contains( string key )
        {
            return m_Data.ContainsKey(  key  ) || m_Parent != null && m_Parent.Contains( key );
        }

        public bool ContainsLocal( string key )
        {
            return m_Data.ContainsKey(key);
        }

        public V Get( string key )
        {
            if ( m_Data.ContainsKey(key) )
            {
                return m_Data[key];
            }

            if ( m_Parent == null )
            {
                return default;
            }

            return m_Parent.Get( key );
        }

        //1. Create Empty/Create from Parent
        //2. Get is Depth First Search
        //3. Set is this only

        public IEnumerator < KeyValuePair < string, V > > GetEnumerator()
        {
            return m_Data.GetEnumerator();
        }

        public string GetFinalName( string name )
        {
            return GetPrefix() + name;
        }

        public V Set( string key, V value )
        {
            return  m_Data[key] = value;
        }

        #endregion

        #region Private

        public IEnumerable< KeyValuePair < string, V > > GetFinalEnumerator() =>
            m_Data.Select( x => new KeyValuePair < string, V >( GetFinalName( x.Key ), x.Value ) );

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IEnumerable ) m_Data ).GetEnumerator();
        }

        private string GetPrefix()
        {
            if ( m_Parent == null )
            {
                return m_Prefix + "_";
            }

            return m_Parent.GetPrefix() + m_Prefix + "_";
        }

        #endregion

    }

}
