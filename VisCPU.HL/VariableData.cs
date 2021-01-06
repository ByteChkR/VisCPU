using VisCPU.HL.TypeSystem;

namespace VisCPU.HL
{

    public struct VariableData : IExternalData
    {

        public ExternalDataType DataType => ExternalDataType.VARIABLE;

        public string GetName()
        {
            return name;
        }

        public string GetFinalName()
        {
            return finalName;
        }

        private readonly string name;
        private readonly string finalName;
        public readonly HLTypeDefinition TypeDefinition;

        public uint Size;

        public readonly string InitContent;

        public VariableData( string name, string finalName, uint dataSize, HLTypeDefinition tdef )
        {
            InitContent = null;
            Size = dataSize;
            this.name = name;
            this.finalName = finalName;
            TypeDefinition = tdef;
        }

        public VariableData( string name, string finalName, string content, HLTypeDefinition tdef )
        {
            this.name = name;
            this.finalName = finalName;
            Size = ( uint ) ( content?.Length ?? 1 );
            InitContent = content;
            TypeDefinition = tdef;
        }

    }

}
