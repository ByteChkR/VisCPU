using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisCPU.HL.Events;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.TypeSystem
{
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
