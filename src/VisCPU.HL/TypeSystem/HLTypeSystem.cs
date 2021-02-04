using System.Collections;
using System.Collections.Generic;
using System.Linq;

using VisCPU.HL.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.TypeSystem
{

    public class HlTypeSystem: IEnumerable <HlTypeDefinition>
    {

        private readonly List < HlTypeDefinition > m_DefinedTypes = new List < HlTypeDefinition >();

        #region Public

        public HlTypeSystem()
        {
            AddItem( new UIntTypeDefinition() );
            AddItem( new FloatTypeDefinition() );
            AddItem( new StringTypeDefinition() );
            AddItem( new HlTypeDefinition( "void", true ) );
        }

        public void Import( HlTypeSystem other )
        {
            foreach ( HlTypeDefinition otherDef in other )
            {
                if ( otherDef.IsPublic && !HasType( otherDef.Name ) )
                    AddItem( otherDef );
            }
        }

        public HlTypeDefinition CreateEmptyType( string name, bool isPublic )
        {
            if ( m_DefinedTypes.Any( x => x.Name == name ) )
            {
                EventManager < ErrorEvent >.SendEvent( new HlTypeRedefinitionEvent( name ) );

                return null;
            }

            HlTypeDefinition def = new HlTypeDefinition( name, isPublic );

            AddItem( def );

            return def;
        }

        public HlTypeDefinition GetOrAdd( string name, bool isPublic )
        {
            return m_DefinedTypes.FirstOrDefault( x => x.Name == name ) ?? CreateEmptyType( name, isPublic);
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

        public IEnumerator < HlTypeDefinition > GetEnumerator()
        {
            return m_DefinedTypes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IEnumerable ) m_DefinedTypes ).GetEnumerator();
        }

    }

}
