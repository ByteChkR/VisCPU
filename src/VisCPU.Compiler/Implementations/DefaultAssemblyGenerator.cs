using System;
using System.Collections.Generic;
using System.Linq;

using VisCPU.Compiler.Assembler;
using VisCPU.Compiler.Assembler.Events;
using VisCPU.Compiler.Compiler;
using VisCPU.Compiler.Linking;
using VisCPU.Compiler.Parser.Tokens;
using VisCPU.Utility;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Compiler.Implementations
{

    public class DefaultAssemblyGenerator : AssemblyGenerator
    {

        #region Public

        public override List < byte > Assemble( LinkerResult result )
        {
            List < byte > instrBytes = new List < byte >();

            AssemblyGeneratorSettings settings = SettingsManager.GetSettings < AssemblyGeneratorSettings >();
            Logger.s_Settings.EnableAll = true;
            Log( "Using format: {0}", settings.Format );

            if ( settings.Format.StartsWith( "v2" ) )
            {
                instrBytes.AddRange( BitConverter.GetBytes( ( uint ) 0 ) );
                instrBytes.AddRange( BitConverter.GetBytes( ( uint ) 1 ) );
                instrBytes.AddRange( BitConverter.GetBytes( settings.GlobalOffset ) );
                instrBytes.AddRange( BitConverter.GetBytes( ( uint ) 0 ) );
            }

            Dictionary < string, AddressItem > consts =
                result.Constants.ApplyOffset( settings.GlobalOffset ).ToDictionary( x => x.Key, x => x.Value );

            Dictionary < string, AddressItem > labels =
                result.Labels.ApplyOffset( settings.GlobalOffset ).ToDictionary( x => x.Key, x => x.Value );

            FileCompilation.ApplyToAllTokens(
                                             result.LinkedBinary,
                                             consts,
                                             new List < uint >()
                                            ); //Apply global constants

            List < uint > indexList = new List < uint >();

            FileCompilation.ApplyToAllTokens( result.LinkedBinary, labels, indexList );

            Dictionary < string, AddressItem > ds =
                result.DataSectionHeader.
                       ApplyOffset(
                                   settings.GlobalOffset +
                                   ( uint ) result.LinkedBinary.Count * CpuSettings.InstructionSize
                                  ).
                       ToDictionary( x => x.Key, x => x.Value );

            result.ApplyDataOffset(
                                   ( int ) ( settings.GlobalOffset +
                                             result.LinkedBinary.Count * CpuSettings.InstructionSize )
                                  );

            FileCompilation.ApplyToAllTokens( result.LinkedBinary, ds, indexList );

            foreach ( KeyValuePair < (int, int), Dictionary < string, AddressItem > > resultHiddenAddressItem in result.
                HiddenConstantItems )
            {
                FileCompilation.ApplyToTokens(
                                              result.LinkedBinary,
                                              resultHiddenAddressItem.Value,
                                              new List < uint >(),
                                              resultHiddenAddressItem.Key.Item1,
                                              resultHiddenAddressItem.Key.Item2
                                             ); //Apply global constants
            }

            foreach ( KeyValuePair < (int, int), Dictionary < string, AddressItem > > resultHiddenAddressItem in result.
                HiddenLabelItems )
            {
                Dictionary < string, AddressItem > hiddenLabels =
                    resultHiddenAddressItem.Value.ApplyOffset( settings.GlobalOffset ).
                                            ToDictionary( x => x.Key, x => x.Value );

                FileCompilation.ApplyToTokens(
                                              result.LinkedBinary,
                                              hiddenLabels,
                                              indexList,
                                              resultHiddenAddressItem.Key.Item1,
                                              resultHiddenAddressItem.Key.Item2
                                             ); //Apply global constants
            }

            foreach ( KeyValuePair < (int, int), Dictionary < string, AddressItem > > resultHiddenAddressItem in result.
                HiddenDataSectionItems )
            {
                Dictionary < string, AddressItem > hds = resultHiddenAddressItem.Value.ApplyOffset(
                         settings.GlobalOffset +
                         ( uint ) result.LinkedBinary.Count *
                         CpuSettings.InstructionSize
                        ).
                    ToDictionary(
                                 x => x.Key,
                                 x => x.Value
                                );

                FileCompilation.ApplyToTokens(
                                              result.LinkedBinary,
                                              hds,
                                              indexList,
                                              resultHiddenAddressItem.Key.Item1,
                                              resultHiddenAddressItem.Key.Item2
                                             ); //Apply global constants
            }

            for ( int i = 0; i < result.LinkedBinary.Count; i++ )
            {
                List < byte > bytes = new List < byte >();
                AToken instr = result.LinkedBinary[i][0];
                IEnumerable < AToken > args = result.LinkedBinary[i].Skip( 1 );

                uint opCode =
                    CpuSettings.InstructionSet.GetInstruction(
                                                              CpuSettings.InstructionSet.GetInstruction(
                                                                   instr.GetValue(),
                                                                   result.LinkedBinary[i].Length - 1
                                                                  )
                                                             );

                bytes.AddRange( BitConverter.GetBytes( opCode ) );

                foreach ( AToken aToken in args )
                {
                    if ( aToken is ValueToken vToken )
                    {
                        bytes.AddRange( BitConverter.GetBytes( vToken.Value ) );
                    }
                    else
                    {
                        EventManager < ErrorEvent >.SendEvent( new TokenRecognitionFailureEvent( aToken.GetValue() ) );
                    }
                }

                if ( bytes.Count > CpuSettings.ByteSize )
                {
                    EventManager < ErrorEvent >.SendEvent( new InvalidArgumentCountEvent( i ) );
                }

                bytes.AddRange( Enumerable.Repeat( ( byte ) 0, ( int ) CpuSettings.ByteSize - bytes.Count ) );

                instrBytes.AddRange( bytes );
            }

            List < byte > v = result.DataSection.SelectMany( BitConverter.GetBytes ).ToList();
            instrBytes.AddRange( v );

            if ( settings.Format.StartsWith( "v3" ) )
            {
                instrBytes.InsertRange( 0, indexList.SelectMany( BitConverter.GetBytes ) );
                instrBytes.InsertRange( 0, BitConverter.GetBytes( 0u ) );
                instrBytes.InsertRange( 0, BitConverter.GetBytes( ( uint ) indexList.Count ) );
                instrBytes.InsertRange( 0, BitConverter.GetBytes( 2u ) );
                instrBytes.InsertRange( 0, BitConverter.GetBytes( 0u ) );
            }

            return instrBytes;
        }

        #endregion

    }

}
