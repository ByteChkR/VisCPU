using System;
using System.Collections.Generic;
using System.Linq;

using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.TypeSystem
{
    public class HLTypeRedefinitionEvent : ErrorEvent
    {

        private const string EVENT_KEY = "hl-type-redefinition";
        public HLTypeRedefinitionEvent(string typeName) : base($"Duplicate definition of type {typeName}", EVENT_KEY, false)
        {
        }

    }
    public class HLTypeSystem
    {
        public HLTypeSystem()
        {
            AddItem( new VarTypeDefinition() );
        }
        private readonly List < HLTypeDefinition > DefinedTypes = new List < HLTypeDefinition >();

        public HLTypeDefinition GetType( string name ) => DefinedTypes.First( x => x.Name == name );

        public HLTypeDefinition GetOrAdd( string name ) =>
            DefinedTypes.FirstOrDefault( x => x.Name == name ) ?? CreateEmptyType( name );
        
        private void AddItem(HLTypeDefinition def)
        {
            DefinedTypes.Add(def);
        }

        public bool HasType( string name )
        {
            return DefinedTypes.Any( x => x.Name == name);
        }
        
        public HLTypeDefinition CreateEmptyType( string name)
        {
            if ( DefinedTypes.Any( x => x.Name == name ) )
            {
                EventManager < ErrorEvent >.SendEvent( new HLTypeRedefinitionEvent( name ) );

                return null;
            }
            HLTypeDefinition def = new HLTypeDefinition(name);

            AddItem( def );

            return def;
        }

    }

}
