namespace VisCPU.Utility
{

    public struct UIntHexNumberString
    {

        public uint Value;

        public UIntHexNumberString( uint val )
        {
            Value = val;
        }

        public override string ToString()
        {
            return Value.ToHexString();
        }

    }

}
