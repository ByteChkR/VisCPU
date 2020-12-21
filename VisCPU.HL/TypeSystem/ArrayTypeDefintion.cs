namespace VisCPU.HL.TypeSystem
{
    public class ArrayTypeDefintion : HLTypeDefinition
    {

        public readonly HLTypeDefinition ElementType;
        public readonly uint Size;
        public ArrayTypeDefintion( HLTypeDefinition elementType, uint size) : base( elementType.Name+"[]" )
        {
            Size = size;
            ElementType = elementType;
        }

        public override uint GetSize()
        {
            return ElementType.GetSize() * Size;
        }

    }
}