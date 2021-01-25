using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.DataTypes
{

    public class LinkedData : IExternalData
    {
        private readonly string m_Name;

        public AddressItem Info { get; }

        public ExternalDataType DataType { get; }

        #region Public

        public LinkedData( string name, AddressItem info, ExternalDataType type )
        {
            m_Name = name;
            Info = info;
            DataType = type;
        }

        public string GetFinalName()
        {
            return m_Name;
        }

        public string GetName()
        {
            return m_Name;
        }

        #endregion
    }

}
