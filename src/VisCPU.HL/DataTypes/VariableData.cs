using System;
using VisCPU.HL.TypeSystem;

namespace VisCPU.HL.DataTypes
{
    [Flags]
    public enum VariableDataEmitFlags
    {
        None = 0,
        CStyle = 1,
        Packed = 2,
        Visible = 4,
        Pointer = 8,
    }

    public struct VariableData : IExternalData, IEquatable < VariableData >
    {
        public ExternalDataType DataType { get; }

        private readonly VariableDataEmitFlags m_EmitFlags;

        public string GetName()
        {
            return m_Name;
        }

        public string GetFinalName()
        {
            return m_FinalName;
        }

        private readonly string m_Name;
        private readonly string m_FinalName;
        public readonly HlTypeDefinition TypeDefinition;

        public bool IsVisible => ( m_EmitFlags & VariableDataEmitFlags.Visible ) != 0;
        public  bool IsPointer => (m_EmitFlags & VariableDataEmitFlags.Pointer) != 0;

        public uint Size { get; }

        public readonly string InitContent;

        public VariableData(
            string name,
            string finalName,
            uint dataSize,
            HlTypeDefinition tdef,
            VariableDataEmitFlags emFlags,
            ExternalDataType dt = ExternalDataType.Variable )
        {
            DataType = dt;
            InitContent = null;
            Size = dataSize;
            m_Name = name;
            m_FinalName = finalName;
            TypeDefinition = tdef;
            m_EmitFlags = emFlags;
        }

        public VariableData(
            string name,
            string finalName,
            string content,
            HlTypeDefinition tdef,
            VariableDataEmitFlags emFlags)
        {
            DataType = ExternalDataType.Variable;
            m_Name = name;
            m_FinalName = finalName;
            Size = ( uint ) ( content?.Length ?? 1 );
            InitContent = content;
            TypeDefinition = tdef;
            m_EmitFlags = emFlags;
        }

        public bool Equals( VariableData other )
        {
            return m_Name == other.m_Name &&
                   m_FinalName == other.m_FinalName &&
                   Equals( TypeDefinition, other.TypeDefinition ) &&
                   IsVisible == other.IsVisible &&
                   Size == other.Size &&
                   InitContent == other.InitContent;
        }

        public override bool Equals( object obj )
        {
            return obj is VariableData other && Equals( other );
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = m_Name != null ? m_Name.GetHashCode() : 0;
                hashCode = ( hashCode * 397 ) ^ ( m_FinalName != null ? m_FinalName.GetHashCode() : 0 );
                hashCode = ( hashCode * 397 ) ^ ( TypeDefinition != null ? TypeDefinition.GetHashCode() : 0 );
                hashCode = ( hashCode * 397 ) ^ IsVisible.GetHashCode();
                hashCode = ( hashCode * 397 ) ^ ( int ) Size;
                hashCode = ( hashCode * 397 ) ^ ( InitContent != null ? InitContent.GetHashCode() : 0 );

                return hashCode;
            }

            
        }
        public string EmitVasm()
        {

            string linkerArgs = "";

            if (IsVisible)
                linkerArgs += "linker:hide ";
            if ((m_EmitFlags & VariableDataEmitFlags.CStyle)!=0)
                linkerArgs += "string:c-style ";
            if ((m_EmitFlags & VariableDataEmitFlags.Packed) != 0)
                linkerArgs += "string:packed ";

            if ( InitContent != null )
                {
                    return
                        $":data {GetFinalName()} \"{InitContent}\" {linkerArgs}";
                }


                return
                    $":data {GetFinalName()} {Size} {linkerArgs}";
        }
    }

}
