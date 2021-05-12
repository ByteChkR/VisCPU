using System.Collections;
using System.Collections.Generic;
using System.Linq;

using VisCPU.HL.Compiler.Events;
using VisCPU.HL.Events;
using VisCPU.HL.Namespaces;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.TypeSystem
{

    public class HlTypeSystem : IEnumerable < HlTypeDefinition >
    {

        private readonly List < HlTypeDefinition > m_DefinedTypes = new List < HlTypeDefinition >();

        #region Public

        public static HlTypeSystem Create( HlNamespace root )
        {
            HlTypeSystem ret = new HlTypeSystem();
            ret.AddItem( new UIntTypeDefinition( root ) );
            ret.AddItem( new FloatTypeDefinition( root ) );
            ret.AddItem( new StringTypeDefinition( root ) );
            ret.AddItem( new CStringTypeDefinition( root ) );
            ret.AddItem( new HlTypeDefinition( root, "void", false, true, true ) );

            return ret;
        }

        public HlTypeDefinition CreateEmptyType(
            HlNamespace ns,
            string name,
            bool isPublic,
            bool isAbstract,
            bool isValueType )
        {
            if ( m_DefinedTypes.Any( x => x.Name == name ) )
            {
                EventManager < ErrorEvent >.SendEvent( new HlTypeRedefinitionEvent( name ) );

                return null;
            }

            HlTypeDefinition def = new HlTypeDefinition( ns, name, isPublic, isAbstract, isValueType );

            AddItem( def );

            return def;
        }

        public void Finalize( HlCompilation compilation )
        {
            m_DefinedTypes.ForEach( x => x.Finalize( compilation ) );
        }

        public IEnumerator < HlTypeDefinition > GetEnumerator()
        {
            return m_DefinedTypes.GetEnumerator();
        }

        public HlTypeDefinition GetType( HlNamespace caller, string name )
        {
            if ( !m_DefinedTypes.Any( x => x.Name == name && x.Namespace.IsVisibleTo( caller ) ) )
            {
                EventManager < ErrorEvent >.SendEvent( new TypeNotFoundEvent( name ) );
            }

            return m_DefinedTypes.First( x => x.Name == name && x.Namespace.IsVisibleTo( caller ) );
        }

        public bool HasType( HlNamespace caller, string name )
        {
            return m_DefinedTypes.Any( x => x.Name == name && x.Namespace.IsVisibleTo( caller ) );
        }

        public void Import( HlNamespace caller, HlTypeSystem other )
        {
            foreach ( HlTypeDefinition otherDef in other )
            {
                if ( otherDef.IsPublic && !HasType( caller, otherDef.Name ) )
                {
                    AddItem( otherDef );
                }
            }
        }

        #endregion

        #region Private

        private HlTypeSystem()
        {
        }

        private void AddItem( HlTypeDefinition def )
        {
            m_DefinedTypes.Add( def );
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IEnumerable ) m_DefinedTypes ).GetEnumerator();
        }

        #endregion

    }

}
