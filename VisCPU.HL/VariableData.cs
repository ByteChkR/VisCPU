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

        public uint Size;

        public readonly string InitContent;

        public VariableData( string name, string finalName, uint dataSize )
        {
            InitContent = null;
            Size = dataSize;
            this.name = name;
            this.finalName = finalName;
        }

        public VariableData( string name, string finalName, string content )
        {
            this.name = name;
            this.finalName = finalName;
            Size = ( uint ) ( content?.Length ?? 1 );
            InitContent = content;
        }

    }

}
