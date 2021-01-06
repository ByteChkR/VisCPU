using System.Collections.Generic;
using System.Linq;

using VisCPU.HL.Events;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.TypeSystem
{

    public class HLTypeSystem
    {

        private readonly List < HLTypeDefinition > DefinedTypes = new List < HLTypeDefinition >();

        #region Public

        public HLTypeSystem()
        {
            AddItem( new VarTypeDefinition() );
        }

        public HLTypeDefinition CreateEmptyType( string name )
        {
            if ( DefinedTypes.Any( x => x.Name == name ) )
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
            return DefinedTypes.FirstOrDefault( x => x.Name == name ) ?? CreateEmptyType( name );
        }

        public HLTypeDefinition GetType( string name )
        {
            return DefinedTypes.First( x => x.Name == name );
        }

        public bool HasType( string name )
        {
            return DefinedTypes.Any( x => x.Name == name );
        }

        #endregion

        #region Private

        private void AddItem( HLTypeDefinition def )
        {
            DefinedTypes.Add( def );
        }

        #endregion

    }

}
