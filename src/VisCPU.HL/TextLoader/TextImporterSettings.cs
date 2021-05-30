using System.Collections.Generic;
using System.Xml.Serialization;

using Newtonsoft.Json;

using VisCPU.Utility.ArgumentParser;

namespace VisCPU.HL.TextLoader
{

    public class TextImporterSettings
    {

        public (string, string)[] DefinedSymbols
        {
            get
            {
                List < (string, string) > ret = new List < (string, string) >();

                foreach ( string definedSymbol in m_DefinedSymbols )
                {
                    string[] p = definedSymbol.Split( '=' );
                    ret.Add( ( p[0], p[1] ) );
                }

                return ret.ToArray();
            }
        }

        [field: Argument( Name = "importer:args" )]
        [field: Argument( Name = "imp:args" )]
        [XmlIgnore]
        [JsonIgnore]
        private string[] m_DefinedSymbols { get; set; } = new string[0];

    }

}