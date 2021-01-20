namespace VisCPU.HL.TypeSystem
{

    public class ArrayTypeDefintion : HlTypeDefinition
    {
        public HlTypeDefinition ElementType { get; }

        public uint Size { get; }

        #region Public

        public ArrayTypeDefintion( HlTypeDefinition elementType, uint size ) : base( elementType.Name + "[]" )
        {
            Size = size;
            ElementType = elementType;
        }

        public override uint GetSize()
        {
            return ElementType.GetSize() * Size;
        }

        #endregion
    }

}
