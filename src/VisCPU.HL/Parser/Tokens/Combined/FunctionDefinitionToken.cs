using System.Collections.Generic;
using System.Linq;

using VisCPU.HL.TypeSystem;

namespace VisCPU.HL.Parser.Tokens.Combined
{

    public class FunctionDefinitionToken : CombinedToken
    {

        public IHlToken[] Arguments { get; }

        public IHlToken[] Block { get; }

        public IHlToken FunctionName { get; }

        public IHlToken FunctionReturnType { get; }

        public IHlToken[] Mods => m_Modifiers.ToArray();

        private List < IHlToken > m_Modifiers;

        public HlTypeDefinition Parent { get; }

        public HlFunctionType FunctionType { get; }

        #region Public

        public FunctionDefinitionToken(
            IHlToken name,
            IHlToken retType,
            IHlToken[] args,
            IHlToken[] mods,
            IHlToken[] subtokens,
            int start,
            HlTypeDefinition parent = null,
            HlFunctionType functionType = HlFunctionType.Function ) : base(
                                                                           HlTokenType.OpFunctionDefinition,
                                                                           subtokens,
                                                                           start
                                                                          )
        {
            FunctionType = functionType;
            Parent = parent;
            FunctionName = name;
            FunctionReturnType = retType;
            m_Modifiers = mods.ToList();
            Arguments = args;
            Block = subtokens;
        }
        public void MakeInternal()
        {
            if (m_Modifiers.All(x => x.Type != HlTokenType.OpInternalMod))
                m_Modifiers.Add(new HlTextToken(HlTokenType.OpInternalMod, "internal", -1));
        }


        #endregion

    }

}
