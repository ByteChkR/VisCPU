using System.Collections.Generic;
using System.Linq;

using VisCPU.HL.Events;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.TypeSystem.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;

namespace VisCPU.HL.TypeSystem
{

    public class HlTypeDefinition : IHlTypeSystemInstance
    {

        private readonly List < HlMemberDefinition > m_Members = new List < HlMemberDefinition >();

        public string Name { get; }

        public int SourceIndex { get; }

        public HlTokenType Type => HlTokenType.OpClassDefinition;

        #region Public

        public HlTypeDefinition( string name )
        {
            Name = name;
        }

        public static uint RecursiveGetOffset( HlTypeDefinition start, uint value, int current, string[] parts )
        {
            uint ret = value + start.GetOffset( parts[current] );

            if ( current == parts.Length - 1 )
            {
                return ret;
            }

            return RecursiveGetOffset(
                                      start.GetType( start.GetPrivateOrPublicMember( parts[current] ) ),
                                      ret,
                                      current + 1,
                                      parts
                                     );
        }

        public static HlMemberDefinition RecursiveGetPrivateOrPublicMember(
            HlTypeDefinition start,
            int current,
            string[] parts )
        {
            HlMemberDefinition ret = start.GetPrivateOrPublicMember( parts[current] );

            if ( current == parts.Length - 1 )
            {
                return ret;
            }

            return RecursiveGetPrivateOrPublicMember(
                                                     start.GetType( start.GetPrivateOrPublicMember( parts[current] ) ),
                                                     current + 1,
                                                     parts
                                                    );
        }

        public static HlMemberDefinition RecursiveGetPublicMember( HlTypeDefinition start, int current, string[] parts )
        {
            HlMemberDefinition ret = start.GetPublicMember( parts[current] );

            if ( current == parts.Length - 1 )
            {
                return ret;
            }

            return RecursiveGetPublicMember(
                                            start.GetType( start.GetPublicMember( parts[current] ) ),
                                            current + 1,
                                            parts
                                           );
        }

        public void AddMember( HlMemberDefinition member )
        {
            if ( m_Members.Any( x => x.Name == member.Name ) )
            {
                EventManager < ErrorEvent >.SendEvent( new HlMemberRedefinitionEvent( member.Name, Name ) );

                return;
            }

            m_Members.Add( member );
        }

        public List < IHlToken > GetChildren()
        {
            return m_Members.Cast < IHlToken >().ToList();
        }

        public uint GetOffset( string name )
        {
            uint ret = 0;

            foreach ( HlMemberDefinition hlMemberDefinition in m_Members )
            {
                if ( hlMemberDefinition.Name == name )
                {
                    return ret;
                }

                ret += hlMemberDefinition.GetSize();
            }

            EventManager < ErrorEvent >.SendEvent( new HlMemberNotFoundEvent( name ) );

            return 0;
        }

        public HlMemberDefinition GetPrivateOrPublicMember( string memberName )
        {
            return m_Members.First( x => x.Name == memberName );
        }

        public HlMemberDefinition GetPublicMember( string memberName )
        {
            return m_Members.First( x => x.IsPublic && x.Name == memberName );
        }

        public virtual uint GetSize()
        {
            return ( uint ) m_Members.Sum( x => x.GetSize() );
        }

        #endregion

        #region Private

        private HlTypeDefinition GetType( HlMemberDefinition def )
        {
            if ( def is HlPropertyDefinition pdef )
            {
                return pdef.PropertyType;
            }

            return null;
        }

        #endregion

    }

}
