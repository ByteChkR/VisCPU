namespace VisCPU.HL.TypeSystem
{

    public class ArrayTypeDefintion : HLTypeDefinition
    {

        public HLTypeDefinition ElementType { get; }

        public uint Size { get; }

        #region Public

        public ArrayTypeDefintion( HLTypeDefinition elementType, uint size ) : base( elementType.Name + "[]" )
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
