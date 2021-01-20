using System.Collections.Generic;
using System.Linq;
using VisCPU.HL.Events;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.TypeSystem
{

    public class HLTypeSystem
    {
        private readonly List < HLTypeDefinition > m_DefinedTypes = new List < HLTypeDefinition >();

        #region Public

        public HLTypeSystem()
        {
            AddItem( new VarTypeDefinition() );
            AddItem( new StringTypeDefinition() );
        }

        public HLTypeDefinition CreateEmptyType( string name )
        {
            if ( m_DefinedTypes.Any( x => x.Name == name ) )
            {
                EventManager < ErrorEvent >.SendEvent( new HLTypeRedefinitionEvent( name ) );

                return null;
            }

            HLTypeDefinition def = new HLTypeDefinition( name );

            AddItem( def );

            return def;
        }

        public HLTypeDefinition GetOrAdd( string name )
        {
            return m_DefinedTypes.FirstOrDefault( x => x.Name == name ) ?? CreateEmptyType( name );
        }

        public HLTypeDefinition GetType( string name )
        {
            return m_DefinedTypes.First( x => x.Name == name );
        }

        public bool HasType( string name )
        {
            return m_DefinedTypes.Any( x => x.Name == name );
        }

        #endregion

        #region Private

        private void AddItem( HLTypeDefinition def )
        {
            m_DefinedTypes.Add( def );
        }

        #endregion
    }

}
