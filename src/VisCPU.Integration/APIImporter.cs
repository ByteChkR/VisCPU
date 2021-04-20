using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using VisCPU.HL;
using VisCPU.HL.DataTypes;
using VisCPU.HL.Importer;
using VisCPU.Utility;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Integration
{

    public class ApiImporter : AImporter, IDataImporter, IFileImporter
    {

        public readonly string DeviceDriverDirectory;
        private readonly Dictionary < uint, FunctionData > m_ExposedApis = new Dictionary < uint, FunctionData >();

        #region Public

        public ApiImporter()
        {
            DeviceDriverDirectory = Path.Combine( CacheDirectory, "api-devices" );

            if ( !Directory.Exists( DeviceDriverDirectory ) )
            {
                Directory.CreateDirectory( DeviceDriverDirectory );
            }
        }

        public void AddApi( uint addr, FunctionData funcData )
        {
            m_ExposedApis.Add( addr, funcData );

            string driverDir = Path.Combine(
                                            DeviceDriverDirectory,
                                            funcData.GetFinalName() + ".vhl"
                                           );

            File.WriteAllText( driverDir, GenerateApiDriver( funcData, addr ) );
        }

        public override bool CanImport( string input )
        {
            return input.StartsWith( "api-integration " );
        }

        #endregion

        #region Private

        private string GenerateApiDriver( FunctionData data, uint devAddr )
        {
            StringBuilder sb = new StringBuilder( "// This is an automatically generated file.\n" );
            sb.Append( "// Do not change unless you know exactly what you are doing.\n" );

            sb.AppendFormat(
                            "// Device Driver for API Call: {0} on Address {1}\n\n",
                            data.GetFinalName(),
                            devAddr.ToHexString()
                           );

            sb.AppendFormat( "public {1} {0}(", data.GetFinalName(), HLBaseTypeNames.s_UintTypeName );

            for ( int i = 0; i < data.ParameterCount; i++ )
            {
                if ( i != 0 )
                {
                    sb.AppendFormat( ", {1} arg{0}", i, HLBaseTypeNames.s_UintTypeName );
                }
                else
                {
                    sb.AppendFormat( "{1} arg{0}", i, HLBaseTypeNames.s_UintTypeName );
                }
            }

            sb.Append( ")\n{\n" );

            sb.AppendFormat( "\t{1} addr = {0};\n", devAddr, HLBaseTypeNames.s_UintTypeName );
            sb.Append( "\treturn addr(" );

            for ( int i = 0; i < data.ParameterCount; i++ )
            {
                if ( i != 0 )
                {
                    sb.AppendFormat( ", arg{0}", i );
                }
                else
                {
                    sb.AppendFormat( "arg{0}", i );
                }
            }

            sb.Append( ");\n}" );

            return sb.ToString();
        }

        IncludedItem IFileImporter.ProcessImport( string input )
        {
            string name = input.Remove( 0, "api-integration ".Length );
            KeyValuePair < uint, FunctionData > api = m_ExposedApis.First( x => x.Value.GetFinalName() == name );

            string target = Path.Combine(
                                         DeviceDriverDirectory,
                                         api.Value.GetFinalName() + ".vhl"
                                        );

            Log( "Including Device Driver: {0} :: {1}", name, target );

            return new IncludedItem( target, false );
        }

        IExternalData[] IDataImporter.ProcessImport( HlCompilation compilation, string input )
        {
            string name = input.Remove( 0, "api-integration ".Length );
            KeyValuePair < uint, FunctionData > api = m_ExposedApis.First( x => x.Value.GetFinalName() == name );

            return new[] { api.Value };
        }

        #endregion

    }

}
