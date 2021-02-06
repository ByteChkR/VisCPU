using System;
using System.Collections;
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

    public class HlTypeDefinition : IHlTypeSystemInstance, IEnumerable <HlMemberDefinition>
    {
        public bool IsPublic { get; }

        private readonly List < HlMemberDefinition > m_Members = new List < HlMemberDefinition >();

        public IEnumerable < HlFunctionDefinition > OverridableFunctions =>
            m_Types.SelectMany( x => x.OverridableFunctions ).
                    Concat(
                           m_Members.Where( x => (x.IsVirtual || x.IsAbstract) && x is HlFunctionDefinition ).
                                     Cast < HlFunctionDefinition >()
                          );

        public string Name { get; }

        public int SourceIndex { get; }

        public bool IsValueType { get; }

        public HlTokenType Type => HlTokenType.OpClassDefinition;

        public HlMemberDefinition StaticConstructor => m_Members.FirstOrDefault(x => x.Type == HlTokenType.OpFunctionDefinition && x.Name == Name && x.IsStatic);
        public HlMemberDefinition StaticDestructor => m_Members.FirstOrDefault(x => x.Type == HlTokenType.OpFunctionDefinition && x.Name == Name && x.IsStatic);
        public HlMemberDefinition DynamicConstructor => m_Members.FirstOrDefault(x => x.Type == HlTokenType.OpFunctionDefinition && x.Name == Name && !x.IsStatic);
        public HlMemberDefinition DynamicDestructor => m_Members.FirstOrDefault(x => x.Type == HlTokenType.OpFunctionDefinition && x.Name == Name && !x.IsStatic);

        private readonly List < IHlToken > m_BaseTypes;
        private List < HlTypeDefinition > m_Types;
        #region Public

        public HlTypeDefinition( string name, bool isPublic, bool isValueType)
        {
            IsValueType = isValueType;
            IsPublic = isPublic;
            Name = name;
            m_BaseTypes = new List <IHlToken>();
        }

        public void Finalize(HlCompilation compilation)
        {

            m_Types = new List<HlTypeDefinition>();
            foreach ( IHlToken baseType in m_BaseTypes )
            {
                HlTypeDefinition def = compilation.TypeSystem.GetType( baseType.ToString() );
                m_Types.Add(def);
            }
        }

        public void AddBaseType(IHlToken token)
        {
            m_BaseTypes.Add(token);
        }

        public string GetFinalMemberName( HlMemberDefinition member )
        {
            if ( member is HlPropertyDefinition )
            {
                if(member.IsStatic)
                return GetFinalStaticProperty(member.Name);

                return GetFinalDynamicProperty( member.Name );
            }
            else
            {
                if (member.IsStatic)
                    return GetFinalStaticFunction(member.Name);
                return GetFinalDynamicFunction(member.Name);
            }
        }

        public string GetFinalStaticFunction(string name)
        {
            if (m_Members.All(x => x.Name != name))
            {
                HlTypeDefinition tdef = m_Types.First(x => x.HasMember(name));
                return tdef.GetFinalStaticFunction(name);
            }
            return $"SFUN_{Name}_{name}";
        }

        public string GetFinalStaticProperty(string name)
        {
            if (m_Members.All(x => x.Name != name))
            {
                HlTypeDefinition tdef = m_Types.First(x => x.HasMember(name));
                return tdef.GetFinalStaticProperty(name);
            }
            return $"SFLD_{Name}_{name}";
        }

        public string GetFinalDynamicFunction(string name)
        {
            if (m_Members.All(x => x.Name != name))
            {
                HlTypeDefinition tdef = m_Types.First(x => x.HasMember(name));
                return tdef.GetFinalDynamicFunction(name);
            }

            HlMemberDefinition mdef = GetPrivateOrPublicMember( name );
            string prefix = mdef.IsAbstract ? "ADFUN_" : mdef.IsVirtual ? "VDFUN_" : "DFUN_";
            return $"{prefix}_{Name}_{name}";
        }

        public string GetFinalDynamicProperty(string name)
        {
            if (m_Members.All(x => x.Name != name))
            {
                HlTypeDefinition tdef = m_Types.First(x => x.HasMember(name));
                return tdef.GetFinalDynamicProperty(name);
            }
            HlMemberDefinition mdef = GetPrivateOrPublicMember(name);
            string prefix = mdef.IsAbstract ? "ADFLD_" : mdef.IsVirtual ? "VDFLD_" : "DFLD_";
            return $"{prefix}_{Name}_{name}";
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

        private bool FindOffsetInBase( Func <HlMemberDefinition, bool> condition, out uint offset )
        {
            uint ret = 0;

            foreach (HlTypeDefinition hlTypeDefinition in m_Types)
            {
                if ( hlTypeDefinition.HasMember( condition ) )
                {
                    uint off = hlTypeDefinition.GetOffset(condition);
                    ret += off;
                    offset = ret;
                    return true;
                }
                else
                {
                    ret += hlTypeDefinition.GetSize();
                }
            }

            offset = 0;
            return false;
        }

        public uint GetOffset( Func < HlMemberDefinition, bool > condition )
        {
            uint ret = (uint)m_Types.Sum(x => x.GetSize());

            foreach (HlMemberDefinition hlMemberDefinition in m_Members)
            {
                if (condition(hlMemberDefinition))
                {
                    return ret;
                }

                ret += hlMemberDefinition.GetSize();
            }

            if (FindOffsetInBase(condition, out ret))
                return ret;
            EventManager<ErrorEvent>.SendEvent(new HlMemberNotFoundEvent(this));

            return GetSize();
        }

        public uint GetOffset( string name )
        {
            return GetOffset( x => x.Name == name );
        }

        public bool HasMember(Func <HlMemberDefinition, bool> condition) => m_Members.Any(condition) || m_Types.Any(x => x.HasMember(condition));

        public bool HasMember( string memberName ) => HasMember( x => x.Name == memberName);

        public HlMemberDefinition GetPrivateOrPublicMember( string memberName )
        {
            HlMemberDefinition ret = m_Members.FirstOrDefault( x => x.Name == memberName );
            if (ret == null)
            {
                HlTypeDefinition def = m_Types.FirstOrDefault(x => x.HasMember(memberName));
                if (def != null) return def.GetPrivateOrPublicMember(memberName);
                EventManager<ErrorEvent>.SendEvent(new HlMemberNotFoundEvent(this, memberName));
            }

            return ret;
        }

        public HlMemberDefinition GetPublicMember( string memberName )
        {
            HlMemberDefinition ret= m_Members.FirstOrDefault( x => x.IsPublic && x.Name == memberName );
            if(ret== null)
            {
                HlTypeDefinition def = m_Types.FirstOrDefault( x => x.HasMember( memberName ) );
                if(def!=null)return def.GetPublicMember(memberName);
                EventManager < ErrorEvent >.SendEvent( new HlMemberNotFoundEvent( this, memberName ) );
            }

            return ret;
        }

        public virtual uint GetSize()
        {
            return (uint)m_Types.Sum(x=>x.GetSize()) + ( uint ) m_Members.Sum( x => x.GetSize() );
        }

        #endregion

        #region Private

        private HlTypeDefinition GetType( HlMemberDefinition def )
        {
            if ( def is HlPropertyDefinition pdef )
            {
                return pdef.PropertyType;
            }

            if ( def is HlFunctionDefinition fdef )
            {
                return fdef.ReturnType;
            }

            return null;
        }

        #endregion

        public IEnumerator < HlMemberDefinition > GetEnumerator()
        {
            return m_Members.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ( ( IEnumerable ) m_Members ).GetEnumerator();
        }

    }

}
