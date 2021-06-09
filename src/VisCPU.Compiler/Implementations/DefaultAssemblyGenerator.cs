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
            
            Log( "Using format: {0}", settings.Format );

            uint emptyVarSize = GetTotalEmptyVarSize(result);

            if (settings.Format.Contains("-ovars"))
            {
                float savedSpace = emptyVarSize / (float)GetTotalVarCount(result);
                Log("Saved Space: {0}%", Math.Round(savedSpace, 2)*100);
            }

            if ( settings.Format.StartsWith( "v2" ) )
            {
                if (settings.Format.Contains("-ovars"))
                {
                    instrBytes.AddRange(BitConverter.GetBytes(emptyVarSize));
                }
                else
                {
                    instrBytes.AddRange( BitConverter.GetBytes(0u));
                }
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

            FixEmptyVars( ( uint ) result.DataSection.Count, CleanEmptyItems( result ) );

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
                if (settings.Format.Contains("-ovars"))
                {
                    instrBytes.InsertRange(0, BitConverter.GetBytes(emptyVarSize));
                }
                else
                {
                    instrBytes.InsertRange(0, BitConverter.GetBytes(0u));
                }

                if ( settings.Format.StartsWith("v3-pic"))
                {
                    List < uint > symbolTable = new List < uint >();
                    symbolTable.Add( ( uint ) result.Labels.Count );

                    foreach ( KeyValuePair < string, AddressItem > keyValuePair in result.Labels )
                    {
                        uint[] bs = keyValuePair.Key.ToCharArray().
                                                 Select( x => ( uint ) x ).
                                                 ToArray();

                        symbolTable.Add( ( uint ) keyValuePair.Key.Length );
                        symbolTable.Add( keyValuePair.Value.Address );
                        symbolTable.AddRange( bs );
                    }

                    instrBytes.InsertRange( 0, symbolTable.SelectMany( BitConverter.GetBytes ) );
                }
            }

            return instrBytes;
        }

        private List < (Dictionary <string,AddressItem>, string, AddressItem) > CleanEmptyItems( LinkerResult result )
        {
            List < (Dictionary<string, AddressItem>, string, AddressItem) > ret = new List < (Dictionary<string, AddressItem>, string, AddressItem) >();
            foreach (string name in result.DataSectionHeader.Keys.ToList())
            {
                AddressItem item = result.DataSectionHeader[name];

                if ( item.IsEmpty )
                {
                    ret.Add( (result.DataSectionHeader, name, item ) );
                    result.DataSectionHeader.Remove(name);
                }
            }
            foreach (KeyValuePair<(int, int), Dictionary<string, AddressItem>> section in result.HiddenDataSectionItems)
            {
                foreach (string name in section.Value.Keys.ToList())
                {
                    AddressItem item = section.Value[name];

                    if (item.IsEmpty)
                    {
                        ret.Add((section.Value,name, item));
                        section.Value.Remove( name );
                    }
                }
            }

            return ret;
        }
        private uint GetTotalVarSize(LinkerResult result)
        {
            uint size = 0;
            foreach (KeyValuePair<string, AddressItem> keyValuePair in result.DataSectionHeader)
            {
                    size += keyValuePair.Value.Size;
            }
            foreach (KeyValuePair<(int, int), Dictionary<string, AddressItem>> section in result.HiddenDataSectionItems)
            {
                foreach (KeyValuePair<string, AddressItem> keyValuePair in section.Value)
                {
                    
                        size += keyValuePair.Value.Size;
                }
            }

            return size;
        }

        private uint GetTotalEmptyVarSize(LinkerResult result)
        {
            uint size = 0;
            foreach (KeyValuePair<string, AddressItem> keyValuePair in result.DataSectionHeader)
            {
                if (keyValuePair.Value.IsEmpty)
                    size += keyValuePair.Value.Size;
            }
            foreach (KeyValuePair<(int, int), Dictionary<string, AddressItem>> section in result.HiddenDataSectionItems)
            {
                foreach (KeyValuePair<string, AddressItem> keyValuePair in section.Value)
                {

                    if (keyValuePair.Value.IsEmpty)
                        size += keyValuePair.Value.Size;
                }
            }

            return size;
        }

        private uint GetTotalVarCount(LinkerResult result)
        {
            uint size = 0;
            foreach (KeyValuePair<string, AddressItem> keyValuePair in result.DataSectionHeader)
            {
                size++;
            }
            foreach (KeyValuePair<(int, int), Dictionary<string, AddressItem>> section in result.HiddenDataSectionItems)
            {
                foreach (KeyValuePair<string, AddressItem> keyValuePair in section.Value)
                {
                    size++;
                }
            }

            return size;
        }

        #endregion

        private static void FixEmptyVars(uint startAddr,
            List < (Dictionary < string, AddressItem >, string, AddressItem) > emptyItems )
            {
                uint currentAddr = startAddr;
            foreach ((Dictionary<string, AddressItem> d, string name, AddressItem item) in emptyItems)
            {
                AddressItem newItem = item;
                newItem.Address = currentAddr;
                currentAddr += newItem.Size;
                d[name] = newItem;
            }
        }
    }

}
