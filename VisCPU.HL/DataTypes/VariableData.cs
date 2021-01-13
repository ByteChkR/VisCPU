using VisCPU.HL.TypeSystem;

namespace VisCPU.HL.DataTypes
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
        public readonly bool IsVisible;

        public uint Size;

        public readonly string InitContent;

        public VariableData( string name, string finalName, uint dataSize, HLTypeDefinition tdef, bool isVisible )
        {
            InitContent = null;
            Size = dataSize;
            this.name = name;
            this.finalName = finalName;
            TypeDefinition = tdef;
            IsVisible = isVisible;
        }

        public VariableData( string name, string finalName, string content, HLTypeDefinition tdef, bool isVisible )
        {
            this.name = name;
            this.finalName = finalName;
            Size = ( uint ) ( content?.Length ?? 1 );
            InitContent = content;
            TypeDefinition = tdef;
            IsVisible = isVisible;
        }

    }

}
