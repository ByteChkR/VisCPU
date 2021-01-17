using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace VisCPU.Utility.Settings.Loader
{

    public class XMLSettingsLoader : SettingsLoader
    {

        private readonly Dictionary < Type, XmlSerializer > m_Serializers = new Dictionary < Type, XmlSerializer >();

        #region Public

        public override object LoadSettings( Type t, string file )
        {
            XmlSerializer s = GetSerializer( t );

            using ( Stream fileStream = File.OpenRead( file ) )
            {
                return s.Deserialize( fileStream );
            }
        }

        public override void SaveSettings( object o, string file )
        {
            XmlSerializer s = GetSerializer( o.GetType() );

            using ( Stream fileStream = File.OpenRead( file ) )
            {
                s.Serialize( fileStream, o );
            }
        }

        #endregion

        #region Private

        private XmlSerializer GetSerializer( Type t )
        {
            if ( m_Serializers.ContainsKey( t ) )
            {
                return m_Serializers[t];
            }

            return m_Serializers[t] = new XmlSerializer( t );
        }

        #endregion

    }

}
