using System;

using VisCPU.HL.TypeSystem;

namespace VisCPU.HL.DataTypes
{

    public struct ConstantData : IExternalData, IEquatable < ConstantData >
    {

        public ExternalDataType DataType => ExternalDataType.Variable;

        public int UseCount { get; private set; }

        public void SetUsed()
        {
            UseCount++;
        }

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
        public readonly string Value;
        public readonly bool IsVisible;

        public readonly string InitContent;

        public ConstantData( string name, string finalName, string value, HlTypeDefinition tdef, bool isVisible )
        {
            UseCount = 0;
            InitContent = null;
            Value = value;
            m_Name = name;
            m_FinalName = finalName;
            TypeDefinition = tdef;
            IsVisible = isVisible;
        }

        public bool Equals( ConstantData other )
        {
            return m_Name == other.m_Name &&
                   m_FinalName == other.m_FinalName &&
                   Equals( TypeDefinition, other.TypeDefinition ) &&
                   IsVisible == other.IsVisible &&
                   Value == other.Value &&
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
                hashCode = ( hashCode * 397 ) ^ Value.GetHashCode();
                hashCode = ( hashCode * 397 ) ^ ( InitContent != null ? InitContent.GetHashCode() : 0 );

                return hashCode;
            }
        }

    }

}
