using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.DataTypes
{

    public class LinkedData : IExternalData
    {

        public readonly AddressItem Info;

        private string Name;

        public ExternalDataType DataType => ExternalDataType.FUNCTION;

        #region Public

        public LinkedData( string name, AddressItem info )
        {
            Name = name;
            Info = info;
        }

        public string GetFinalName()
        {
            return Name;
        }

        public string GetName()
        {
            return Name;
        }

        #endregion

    }

}
