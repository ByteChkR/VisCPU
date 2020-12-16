using System;
using System.Collections.Generic;
using System.Linq;

using VisCPU.Compiler.Assembler.Events;
using VisCPU.Compiler.Compiler;
using VisCPU.Compiler.Linking;
using VisCPU.Compiler.Parser.Tokens;
using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;

namespace VisCPU.Compiler.Assembler
{

    public class DefaultAssemblyGenerator : AssemblyGenerator
    {

        #region Public

        public override List < byte > Assemble( LinkerResult result )
        {
            List < byte > instrBytes = new List < byte >();

            FileCompilation.ApplyToAllTokens( result.LinkedBinary, result.Constants ); //Apply global constants

            FileCompilation.ApplyToAllTokens( result.LinkedBinary, result.Labels );

            Dictionary < string, AddressItem > ds =
                result.DataSectionHeader.
                       ApplyOffset( ( uint ) result.LinkedBinary.Count * CPUSettings.INSTRUCTION_SIZE ).
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
                FileCompilation.ApplyToTokens(
                                              result.LinkedBinary,
                                              resultHiddenAddressItem.Value,
                                              resultHiddenAddressItem.Key.Item1,
                                              resultHiddenAddressItem.Key.Item2
                                             ); //Apply global constants
            }

            foreach ( KeyValuePair < (int, int), Dictionary < string, AddressItem > > resultHiddenAddressItem in result.
                HiddenDataSectionItems )
            {
                Dictionary < string, AddressItem > hds = resultHiddenAddressItem.Value.ApplyOffset(
                         ( uint ) result.LinkedBinary.Count *
                         CPUSettings.INSTRUCTION_SIZE
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

                if ( bytes.Count > CPUSettings.BYTE_SIZE )
                {
                    EventManager < ErrorEvent >.SendEvent( new InvalidArgumentCountEvent( i ) );
                }

                bytes.AddRange( Enumerable.Repeat( ( byte ) 0, CPUSettings.BYTE_SIZE - bytes.Count ) );

                instrBytes.AddRange( bytes );
            }

            instrBytes.AddRange( result.DataSection.SelectMany( BitConverter.GetBytes ) );

            return instrBytes;
        }

        #endregion

    }

}
