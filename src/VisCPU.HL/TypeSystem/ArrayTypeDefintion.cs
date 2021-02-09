using VisCPU.HL.Namespaces;

namespace VisCPU.HL.TypeSystem
{

    public class ArrayTypeDefintion : HlTypeDefinition
    {

        public HlTypeDefinition ElementType { get; }

        public uint Size { get; }

        #region Public

        public ArrayTypeDefintion(HlNamespace root, HlTypeDefinition elementType, uint size ) : base(root,
             elementType.Name + "[]",
             true,
             false
            )
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
