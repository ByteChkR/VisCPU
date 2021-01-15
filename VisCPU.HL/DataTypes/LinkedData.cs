using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.DataTypes
{

    public class LinkedData : IExternalData
    {

        public readonly AddressItem Info;

        private string m_Name;

        public ExternalDataType DataType => ExternalDataType.Function;

        #region Public

        public LinkedData( string name, AddressItem info )
        {
            m_Name = name;
            Info = info;
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
