using System.Collections.Generic;
using System.Linq;

using VisCPU.HL.Events;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.TypeSystem.Events;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.TypeSystem
{

    public class HLTypeDefinition : IHLTypeSystemInstance
    {

        private readonly List < HLMemberDefinition > Members = new List < HLMemberDefinition >();

        public string Name { get; }

        public int SourceIndex { get; }

        public HLTokenType Type => HLTokenType.OpClassDefinition;

        #region Public

        public HLTypeDefinition( string name )
        {
            Name = name;
        }

        public void AddMember( HLMemberDefinition member )
        {
            if ( Members.Any( x => x.Name == member.Name ) )
            {
                EventManager < ErrorEvent >.SendEvent( new HLMemberRedefinitionEvent( member.Name, Name ) );

                return;
            }

            Members.Add( member );
        }

        public List < IHLToken > GetChildren()
        {
            return Members.Cast < IHLToken >().ToList();
        }


        public HLMemberDefinition GetPrivateOrPublicMember(string memberName)
        {
            return Members.First(x => x.Name == memberName);
        }

        public HLMemberDefinition GetPublicMember( string memberName )
        {
            return Members.First( x => x.IsPublic && x.Name == memberName );
        }

        public static HLMemberDefinition RecursiveGetPrivateOrPublicMember(HLTypeDefinition start, int current, string[] parts)
        {
            HLMemberDefinition ret = start.GetPrivateOrPublicMember(parts[current]);

            if (current == parts.Length - 1)
                return ret;

            return RecursiveGetPrivateOrPublicMember(
                                      start.GetType(start.GetPrivateOrPublicMember(parts[current])),
                                      current + 1,
                                      parts
                                     );
        }
        public static HLMemberDefinition RecursiveGetPublicMember(HLTypeDefinition start, int current, string[] parts)
        {
            HLMemberDefinition ret = start.GetPublicMember(parts[current]);

            if (current == parts.Length - 1)
                return ret;

            return RecursiveGetPublicMember(
                                      start.GetType(start.GetPublicMember(parts[current])),
                                      current + 1,
                                      parts
                                     );
        }

        public static uint RecursiveGetOffset(HLTypeDefinition start, uint value, int current, string[] parts)
        {
            uint ret = value + start.GetOffset( parts[current] );
            
            if(current == parts.Length-1)
                return ret;

            return RecursiveGetOffset(
                                      start.GetType( start.GetPrivateOrPublicMember( parts[current] ) ),
                                      ret,
                                      current + 1,
                                      parts
                                     );
        }

        private HLTypeDefinition GetType(HLMemberDefinition def)
        {
            if ( def is HLPropertyDefinition pdef )
                return pdef.PropertyType;

            return null;
        }

    public uint GetOffset( string name )
        {
            uint ret = 0;

            foreach ( HLMemberDefinition hlMemberDefinition in Members )
            {
                if ( hlMemberDefinition.Name == name )
                {
                    return ret;
                }

                ret += hlMemberDefinition.GetSize();
            }

            EventManager < ErrorEvent >.SendEvent( new HLMemberNotFoundEvent( name ) );

            return 0;
        }

        public virtual uint GetSize()
        {
            return ( uint ) Members.Sum( x => x.GetSize() );
        }

        #endregion

    }

}
