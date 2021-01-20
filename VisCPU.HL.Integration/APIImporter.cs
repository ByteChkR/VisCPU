﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VisCPU.HL.DataTypes;
using VisCPU.HL.Importer;

namespace VisCPU.HL.Integration
{

    public class APIImporter : AImporter, IDataImporter, IFileImporter
    {
        public readonly string DeviceDriverDirectory;
        private readonly Dictionary < uint, FunctionData > m_ExposedApis = new Dictionary < uint, FunctionData >();

        #region Public

        public APIImporter()
        {
            DeviceDriverDirectory = Path.Combine( CacheRoot, "api-devices" );

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
                funcData.GetFinalName() + ".vhl" );

            File.WriteAllText( driverDir, GenerateAPIDriver( funcData, addr ) );
        }

        public override bool CanImport( string input )
        {
            return input.StartsWith( "api-integration " );
        }

        #endregion

        #region Private

        private string GenerateAPIDriver( FunctionData data, uint devAddr )
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat( "public var {0}(", data.GetFinalName() );

            for ( int i = 0; i < data.ParameterCount; i++ )
            {
                if ( i != 0 )
                {
                    sb.AppendFormat( ", var arg{0}", i );
                }
                else
                {
                    sb.AppendFormat( "var arg{0}", i );
                }
            }

            sb.Append( ")" );

            sb.AppendFormat( "var addr = {0};\n", devAddr );
            sb.AppendFormat( "return addr(" );

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

        string IFileImporter.ProcessImport( string input )
        {
            string name = input.Remove( 0, "api-integration ".Length );
            KeyValuePair < uint, FunctionData > api = m_ExposedApis.First( x => x.Value.GetFinalName() == name );

            string target = Path.Combine(
                DeviceDriverDirectory,
                api.Value.GetFinalName() + ".vhl" );

            Log( "Including Device Driver: {0} :: {1}", name, target );

            return target;
        }

        IExternalData[] IDataImporter.ProcessImport( string input )
        {
            string name = input.Remove( 0, "api-integration ".Length );
            KeyValuePair < uint, FunctionData > api = m_ExposedApis.First( x => x.Value.GetFinalName() == name );

            return new[] { api.Value };
        }

        #endregion
    }

}
