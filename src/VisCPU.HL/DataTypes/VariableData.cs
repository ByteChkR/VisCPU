﻿using System;
using VisCPU.HL.TypeSystem;

namespace VisCPU.HL.DataTypes
{

    public struct VariableData : IExternalData, IEquatable < VariableData >
    {
        public ExternalDataType DataType { get; }

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
        public readonly bool IsVisible;
        public readonly bool IsPointer;

        public uint Size { get; }

        public readonly string InitContent;

        public VariableData(
            string name,
            string finalName,
            uint dataSize,
            HlTypeDefinition tdef,
            bool isVisible,
            bool isPointer,
            ExternalDataType dt = ExternalDataType.Variable )
        {
            DataType = dt;
            InitContent = null;
            Size = dataSize;
            m_Name = name;
            m_FinalName = finalName;
            TypeDefinition = tdef;
            IsVisible = isVisible;
            IsPointer = isPointer;
        }

        public VariableData(
            string name,
            string finalName,
            string content,
            HlTypeDefinition tdef,
            bool isVisible,
            bool isPointer )
        {
            DataType = ExternalDataType.Variable;
            m_Name = name;
            m_FinalName = finalName;
            Size = ( uint ) ( content?.Length ?? 1 );
            InitContent = content;
            TypeDefinition = tdef;
            IsVisible = isVisible;
            IsPointer = isPointer;
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
    }

}
