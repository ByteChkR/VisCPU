using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        private HlTypeSystem()
        {
        }

        public static HlTypeSystem Create( HlNamespace root )
        {
            HlTypeSystem ret = new HlTypeSystem();
            ret.AddItem(new UIntTypeDefinition(root));
            ret.AddItem(new FloatTypeDefinition(root));
            ret.AddItem(new StringTypeDefinition(root));
            ret.AddItem(new HlTypeDefinition(root,"void", true, true));

            return ret;
        }

        public HlTypeDefinition CreateEmptyType(HlNamespace ns, string name, bool isPublic, bool isValueType )
        {
            if ( m_DefinedTypes.Any( x => x.Name == name ) )
            {
                EventManager < ErrorEvent >.SendEvent( new HlTypeRedefinitionEvent( name ) );

                return null;
            }

            HlTypeDefinition def = new HlTypeDefinition(ns, name, isPublic, isValueType );

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

        public HlTypeDefinition GetType(HlNamespace caller, string name )
        {
            return m_DefinedTypes.First( x => x.Name == name && x.Namespace.IsVisibleTo(caller));
        }

        public bool HasType(HlNamespace caller, string name )
        {
            return m_DefinedTypes.Any( x => x.Name == name && x.Namespace.IsVisibleTo(caller) );
        }

        public void Import(HlNamespace caller, HlTypeSystem other )
        {
            foreach ( HlTypeDefinition otherDef in other )
            {
                if ( otherDef.IsPublic && !HasType(caller, otherDef.Name ) )
                {
                    AddItem( otherDef );
                }
            }
        }

        #endregion

        #region Private

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
