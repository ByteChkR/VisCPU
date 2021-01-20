using System.Collections.Generic;
using System.Linq;
using VisCPU.HL.Events;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.TypeSystem
{

    public class HlTypeSystem
    {
        private readonly List < HlTypeDefinition > m_DefinedTypes = new List < HlTypeDefinition >();

        #region Public

        public HlTypeSystem()
        {
            AddItem( new VarTypeDefinition() );
            AddItem( new StringTypeDefinition() );
        }

        public HlTypeDefinition CreateEmptyType( string name )
        {
            if ( m_DefinedTypes.Any( x => x.Name == name ) )
            {
                EventManager < ErrorEvent >.SendEvent( new HlTypeRedefinitionEvent( name ) );

                return null;
            }

            HlTypeDefinition def = new HlTypeDefinition( name );

            AddItem( def );

            return def;
        }

        public HlTypeDefinition GetOrAdd( string name )
        {
            return m_DefinedTypes.FirstOrDefault( x => x.Name == name ) ?? CreateEmptyType( name );
        }

        public HlTypeDefinition GetType( string name )
        {
            return m_DefinedTypes.First( x => x.Name == name );
        }

        public bool HasType( string name )
        {
            return m_DefinedTypes.Any( x => x.Name == name );
        }

        #endregion

        #region Private

        private void AddItem( HlTypeDefinition def )
        {
            m_DefinedTypes.Add( def );
        }

        #endregion
    }

}
