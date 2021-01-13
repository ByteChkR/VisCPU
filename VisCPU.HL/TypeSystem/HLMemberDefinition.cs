using System;
using System.Collections.Generic;
using System.Linq;

using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.TypeSystem
{

    public abstract class InvalidHLMemberModifiers : ErrorEvent
    {

        protected InvalidHLMemberModifiers( HLTokenType a, HLTokenType b) : base( $"Token '{a}' can not be used together with '{b}'", ErrorEventKeys.HL_INVALID_MEMBER_MODIFIERS, false )
        {
        }

    }
    public abstract class HLMemberDefinition : IHLTypeSystemInstance
    {
        public readonly bool IsPublic;
        public readonly bool IsStatic;
        public readonly bool IsConstant;
        

        public string Name { get; }

        public int SourceIndex { get; }

        public abstract HLTokenType Type { get; }

        #region Public

        public abstract uint GetSize();

        public List < IHLToken > GetChildren()
        {
            return null;
        }

        #endregion

        #region Protected
        
        protected HLMemberDefinition( string name, IHLToken[] mods)
        {
            Name = name;

            IsConstant = mods.Any(x => x.Type == HLTokenType.OpConstMod);
            IsStatic = mods.Any(x => x.Type == HLTokenType.OpStaticMod);
            IsPublic = mods.Any(x => x.Type == HLTokenType.OpPublicMod);


        }


        #endregion

    }

}
