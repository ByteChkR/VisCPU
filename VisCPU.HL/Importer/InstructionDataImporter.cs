﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using VisCPU.HL.Importer.Events;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.HL.Importer
{

    public class InstructionDataImporter : AImporter, IDataImporter, IFileImporter
    {

        private InstructionSet InstructionSet;

        private string InstructionDirectory => Path.Combine( CacheDirectory, "instruction-src" );

        #region Public

        public InstructionDataImporter( InstructionSet set )
        {
            Directory.CreateDirectory( InstructionDirectory );
            InstructionSet = set;
        }

        public override bool CanImport( string input )
        {
            return input.StartsWith( "vasm-bridge" );
        }

        #endregion

        #region Private

        private string[] GenerateInstructionData( Instruction target )
        {
            List < string > data = new List < string >();
            Log( $"Generating Function for Instruction {target.Key}" );
            data.Add( $"; Generated vasm file for instruction: {target.Key}" );

            data.Add( $":data tmp_ret 0x01 linker:hide" );

            for ( int i = 0; i < target.ArgumentCount; i++ )
            {
                data.Add( $":data arg_{i} 0x01 linker:hide" );
                data.Add( $":data arg_{i}_v 0x01 linker:hide" );
            }

            data.Add( $".I{target.ArgumentCount}_{target.Key}" );

            for ( int i = 0; i < target.ArgumentCount; i++ )
            {
                data.Add( $"POP arg_{i}" );
                data.Add( $"DREF arg_{i} arg_{i}_v" );
            }

            StringBuilder sb = new StringBuilder();
            sb.Append( $"{target.Key}" );

            for ( int i = 0; i < target.ArgumentCount; i++ )
            {
                sb.Append( $" arg_{i}_v" );
            }

            data.Add( sb.ToString() );

            for ( int i = 0; i < target.ArgumentCount; i++ )
            {
                data.Add( $"LOAD tmp_ret arg_{i}_v" );
                data.Add( $"CREF tmp_ret arg_{i}" );
            }

            data.Add( $"LOAD tmp_ret 0" );
            data.Add( $"PUSH tmp_ret" );
            data.Add( $"RET" );

            return data.ToArray();
        }

        private Instruction Parse( string input )
        {
            int tagLen = "vasm-bridge".Length + 1;

            if ( input.Length < tagLen )
            {
                EventManager < ErrorEvent >.SendEvent( new InvalidVasmBridgeArgumentsEvent( input ) );

                return null;
            }

            string cmd = input.Remove( 0, tagLen );

            if ( cmd == "all" )
            {
                return null;
            }

            int argCount = -1;

            if ( cmd.Contains( ' ' ) )
            {
                string[] s = cmd.Split( ' ' );
                argCount = int.Parse( s[1] );
                cmd = s[0];
            }

            Instruction[] iis = InstructionSet.GetInstructions( cmd );

            return argCount != -1 ? iis.First( x => x.ArgumentCount == argCount ) : iis.First();
        }

        string IFileImporter.ProcessImport( string input )
        {
            Instruction target = Parse( input );

            if ( target == null )
            {
                string allPath = Path.Combine( InstructionDirectory, $"all.vasm" );

                if ( !File.Exists( allPath ) )
                {
                    List < string > data = new List < string >();

                    foreach ( Instruction instruction in InstructionSet.GetInstructions() )
                    {
                        data.Add(
                                 $":include {( this as IFileImporter ).ProcessImport( $"vasm-bridge {instruction.Key} {instruction.ArgumentCount}" )}"
                                );
                    }

                    File.WriteAllLines( allPath, data );
                }

                return allPath;
            }

            string path = Path.Combine( InstructionDirectory, $"{target.Key}_{target.ArgumentCount}.vasm" );

            if ( !File.Exists( path ) )
            {
                File.WriteAllLines( path, GenerateInstructionData( target ) );
            }

            return path;
        }

        IExternalData[] IDataImporter.ProcessImport( string input )
        {
            Instruction target = Parse( input );

            if ( target == null )
            {
                List < IExternalData > data = new List < IExternalData >();

                foreach ( Instruction instruction in InstructionSet.GetInstructions() )
                {
                    data.Add(
                             new FunctionData(
                                              $"I{instruction.ArgumentCount}_{instruction.Key}",
                                              true,
                                              null,
                                              ( int ) instruction.ArgumentCount,
                                              true
                                             )
                            );
                }

                return data.ToArray();
            }

            IExternalData d = new FunctionData(
                                               $"I{target.ArgumentCount}_{target.Key}",
                                               true,
                                               null,
                                               ( int ) target.ArgumentCount,
                                               true
                                              );

            return new[] { d };
        }

        #endregion

    }

}