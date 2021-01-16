using System;
using System.Collections.Generic;
using System.Linq;

using VisCPU.Compiler.Assembler;
using VisCPU.Compiler.Assembler.Events;
using VisCPU.Compiler.Compiler;
using VisCPU.Compiler.Linking;
using VisCPU.Compiler.Parser.Tokens;
using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Settings;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Compiler.Implementations
{

    public class DefaultAssemblyGenerator : AssemblyGenerator
    {

        #region Public

        public override List < byte > Assemble( LinkerResult result )
        {
            List < byte > instrBytes = new List < byte >();

            AssemblyGeneratorSettings settings = SettingsSystem.GetSettings<AssemblyGeneratorSettings>();

            if ( settings.GlobalOffset != 0 )
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

            FileCompilation.ApplyToAllTokens( result.LinkedBinary, consts ); //Apply global constants

            FileCompilation.ApplyToAllTokens( result.LinkedBinary, labels );

            Dictionary < string, AddressItem > ds =
                result.DataSectionHeader.
                       ApplyOffset(
                                   settings.GlobalOffset +
                                   ( uint ) result.LinkedBinary.Count * CPUSettings.s_InstructionSize
                                  ).
                       ToDictionary( x => x.Key, x => x.Value );

            FileCompilation.ApplyToAllTokens( result.LinkedBinary, ds );

            foreach ( KeyValuePair < (int, int), Dictionary < string, AddressItem > > resultHiddenAddressItem in result.
                HiddenConstantItems )
            {
                FileCompilation.ApplyToTokens(
                                              result.LinkedBinary,
                                              resultHiddenAddressItem.Value,
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
                         CPUSettings.s_InstructionSize
                        ).
                    ToDictionary(
                                 x => x.Key,
                                 x => x.Value
                                );

                FileCompilation.ApplyToTokens(
                                              result.LinkedBinary,
                                              hds,
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
                    CPUSettings.InstructionSet.GetInstruction(
                                                              CPUSettings.InstructionSet.GetInstruction(
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

                if ( bytes.Count > CPUSettings.s_ByteSize )
                {
                    EventManager < ErrorEvent >.SendEvent( new InvalidArgumentCountEvent( i ) );
                }

                bytes.AddRange( Enumerable.Repeat( ( byte ) 0, ( int ) CPUSettings.s_ByteSize - bytes.Count ) );

                instrBytes.AddRange( bytes );
            }

            instrBytes.AddRange( result.DataSection.SelectMany( BitConverter.GetBytes ) );

            return instrBytes;
        }

        #endregion

    }

}
