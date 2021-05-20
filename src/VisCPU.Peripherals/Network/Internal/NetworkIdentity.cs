namespace VisCPU.Peripherals.Network
{

    public struct NetworkIdentity
    {

        public int NetworkAddress { get; set; }

        public int Port { get; set; }

        public override string ToString()
        {
            return $"{NetworkAddress}:{Port}";
        }

    }

}
