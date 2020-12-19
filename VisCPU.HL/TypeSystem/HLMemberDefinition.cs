﻿using System.Collections.Generic;

using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;

namespace VisCPU.HL.TypeSystem
{

    public abstract class HLMemberDefinition: IHLTypeSystemInstance
    {
        public string Name { get; }

        public abstract uint GetSize();

        protected HLMemberDefinition( string name )
        {
            Name = name;
        }

        public List < IHLToken > GetChildren()
        {
            return null;
        }

        public int SourceIndex { get; }

        public abstract HLTokenType Type { get; }

    }

}