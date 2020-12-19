using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.TypeSystem
{

    public class HLMemberRedefinitionEvent : ErrorEvent
    {

        private const string EVENT_KEY = "hl-member-redefinition";
        public HLMemberRedefinitionEvent(string memberName, string typeName) : base($"Duplicate definition of {memberName} in type {typeName}", EVENT_KEY, true)
        {
        }

    }

    public class VarTypeDefinition : HLTypeDefinition
    {

        public override uint GetSize()
        {
            return 1;
        }

        public VarTypeDefinition() : base( "var" )
        {
        }

    }
    
    public class ArrayTypeDefintion : HLTypeDefinition
    {

        public readonly HLTypeDefinition ElementType;
        public readonly uint Size;
        public ArrayTypeDefintion( HLTypeDefinition elementType, uint size) : base( elementType.Name+"[]" )
        {
            Size = size;
            ElementType = elementType;
        }

        public override uint GetSize()
        {
            return ElementType.GetSize() * Size;
        }

    }
    
    public class HLTypeDefinition : IHLTypeSystemInstance
    {

        public string Name { get; }

        public virtual uint GetSize()
        {
            return (uint)Members.Sum(x=>x.GetSize());
        }
        
        public uint GetOffset(string name)
        {
            uint ret = 0;
            foreach ( HLMemberDefinition hlMemberDefinition in Members )
            {
                if (hlMemberDefinition.Name == name) return ret;

                ret += hlMemberDefinition.GetSize();
            }

            throw new Exception();
        }

        private readonly List < HLMemberDefinition > Members = new List < HLMemberDefinition >();

        public HLTypeDefinition(string name)
        {
            Name = name;
        }

        public HLMemberDefinition GetMember(string memberName) => Members.First(x => x.Name == memberName);
        public void AddMember(HLMemberDefinition member)
        {
            if (Members.Any(x => x.Name == member.Name))
            {
                EventManager<ErrorEvent>.SendEvent(new HLMemberRedefinitionEvent(member.Name, Name));

                return;
            }

            Members.Add(member);
        }

        public List<IHLToken> GetChildren()
        {
            return Members.Cast<IHLToken>().ToList();
        }

        public int SourceIndex { get; }

        public HLTokenType Type => HLTokenType.OpClassDefinition;

    }

}
