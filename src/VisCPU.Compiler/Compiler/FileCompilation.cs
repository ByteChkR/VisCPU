﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using VisCPU.Compiler.Assembler;
using VisCPU.Compiler.Compiler.Events;
using VisCPU.Compiler.Linking;
using VisCPU.Compiler.Parser;
using VisCPU.Compiler.Parser.Tokens;
using VisCPU.Utility;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.IO.Settings;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Compiler.Compiler
{

    public class FileCompilation : VisBase
    {

        public readonly Dictionary < string, AddressItem > Constants = new Dictionary < string, AddressItem >();

        public readonly Dictionary < string, AddressItem > DataSectionHeader = new Dictionary < string, AddressItem >();
        public readonly List < FileReference > FileReferences = new List < FileReference >();
        public readonly Dictionary < string, AddressItem > Labels = new Dictionary < string, AddressItem >();
        public readonly LinkerInfo LinkerInfo;
        public readonly FileReference Reference;

        public readonly string Source;
        public readonly List < AToken[] > Tokens;

        public List < uint > DataSection { get; } = new List < uint >();

        protected override LoggerSystems SubSystem => LoggerSystems.FileCompilation;

        #region Public

        public FileCompilation( LinkerInfo info )
        {
            LinkerInfo = info;
            Source = info.Source;
            Reference = new FileReference( "linked:file" );
            Constants = info.Constants;
            Labels = info.Labels;
            DataSectionHeader = info.DataSectionHeader;
            Tokens = new List < AToken[] >();
        }

        public FileCompilation( FileReference file )
        {
            Reference = file;
            Source = File.ReadAllText( Reference.File );
            Tokens = Tokenizer.Tokenize( Source );
            ResolveConstantItems();
            ProcessFileReferences();

            AssemblyGeneratorSettings s = SettingsManager.GetSettings < AssemblyGeneratorSettings >();
            if (s.Format.Contains("-ovars"))
            {
                (string name, AddressItem item)[] emptyItems = CreateDataSection_Reduced();

                uint currentAddr = (uint)DataSection.Count;
                for ( int i = 0; i < emptyItems.Length; i++ )
                {
                    (string name, AddressItem item) = emptyItems[i];
                    item.Address = currentAddr;
                    item.IsEmpty = true;
                    DataSectionHeader[name] = item;
                    currentAddr += item.Size;
                }
            }
            else
            {
                CreateDataSection_Full();
            }
            ProcessLabels();
        }

        public static void ApplyToAllTokens(
            List < AToken[] > tokens,
            IDictionary < string, AddressItem > header,
            List < uint > indexList )
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                for ( int j = 1; j < tokens[i].Length; j++ )
                {
                    string w = tokens[i][j].GetValue();

                    if ( header.ContainsKey( w ) )
                    {
                        indexList.Add( ( uint ) ( i * 4 + j ) );

                        tokens[i][j] = new ValToken(
                                                    tokens[i][j].OriginalText,
                                                    tokens[i][j].Start,
                                                    tokens[i][j].Length,
                                                    header[w].Address
                                                   );
                    }
                }
            }
        }

        public static void ApplyToTokens(
            List < AToken[] > tokens,
            IDictionary < string, AddressItem > header,
            List < uint > indexList,
            int start,
            int length )
        {
            for ( int i = start; i < start + length; i++ )
            {
                for ( int j = 1; j < tokens[i].Length; j++ )
                {
                    string w = tokens[i][j].GetValue();

                    if ( header.ContainsKey( w ) )
                    {
                        indexList.Add( ( uint ) ( i * 4 + j ) );

                        tokens[i][j] = new ValToken(
                                                    tokens[i][j].OriginalText,
                                                    tokens[i][j].Start,
                                                    tokens[i][j].Length,
                                                    header[w].Address
                                                   );
                    }
                }
            }
        }

        #endregion

        #region Private

        private void AppendFileSection()
        {
            for ( int i = 0; i < Tokens.Count; i++ )
            {
                if ( Tokens[i].Length >= 3 &&
                     Tokens[i][1] is WordToken name &&
                     Tokens[i][0] is WordToken word &&
                     word.GetValue() == ":file" )
                {
                    object[] linkerArgs = ParseLinkerArgs( Tokens[i].Skip( 3 ) );

                    AddressItem item = new AddressItem
                                                         {
                                                             Address = ( uint ) DataSection.Count,
                                                             LinkerArguments = linkerArgs,
                                                             IsEmpty = false,
                                                         };

                    WordToken tFile = Tokens[i][2] as WordToken;

                    string file = Path.Combine(
                                               Path.GetDirectoryName( Reference.File ),
                                               Tokens[i][2] is StringToken str ? str.GetContent() : tFile.GetValue()
                                              );

                    if ( !File.Exists( file ) )
                    {
                        EventManager < ErrorEvent >.SendEvent( new FileNotFoundEvent( file, false ) );
                    }

                    string p = Tokens[i][2] is StringToken st ? st.GetContent() : Tokens[i][2].ToString();

                    uint[] data = File.ReadAllBytes( p ).ToUInt();
                    item.Size = (uint)data.Length;
                    DataSectionHeader[name.GetValue()] = item;
                    DataSection.AddRange( data );

                    DataSectionHeader[name.GetValue() + "_LEN"] = new AddressItem
                                                                  {
                                                                      Address = ( uint ) DataSection.Count,
                                                                      LinkerArguments =
                                                                          new[]
                                                                          {
                                                                              "linker:autogen", "linker:file_length"
                                                                          },
                                                                      IsEmpty = false,
                                                                      Size = 1
                                                                  };

                    DataSection.Add( ( uint ) data.Length );

                    Tokens.RemoveAt( i );
                    i--;
                }
            }
        }

        private (string, AddressItem)[] CreateDataSection_Reduced()
        {
            List < (string, AddressItem) > emptyItems = new List < (string, AddressItem) >();
            for (int i = 0; i < Tokens.Count; i++)
            {
                if (Tokens[i].Length >= 3 &&
                     Tokens[i][1] is WordToken name &&
                     Tokens[i][0] is WordToken word &&
                     word.GetValue() == ":data")
                {
                    object[] linkerArgs = ParseLinkerArgs(Tokens[i].Skip(3));

                    AddressItem item = new AddressItem
                    {
                        Address = (uint)DataSection.Count,
                        LinkerArguments = linkerArgs,
                        IsEmpty = false
                    };

                    if (Tokens[i][2] is StringToken str)
                    {
                        bool compress = linkerArgs.Contains("string:packed");
                        bool nullTerm = linkerArgs.Contains("string:c-style");
                        string data = str.GetContent();

                        if (nullTerm)
                        {
                            data = data + '\0';
                        }

                        if (compress)
                        {
                            List<byte> cData = Encoding.ASCII.GetBytes(data).ToList();
                            cData.AddRange(Enumerable.Repeat((byte)0, cData.Count % sizeof(uint)));

                            item.Size = (uint)(cData.Count / sizeof(uint));

                            for (int j = 0; j < cData.Count / sizeof(uint); j++)
                            {
                                DataSection.Add(BitConverter.ToUInt32(cData.ToArray(), j));
                            }
                        }
                        else
                        {
                            item.Size = ( uint ) data.Length;
                            DataSection.AddRange(data.Select(x => (uint)x));
                        }
                    }
                    else if (Tokens[i][2] is ValueToken val)
                    {
                        uint defVal = Tokens[i].Length == 4 && Tokens[i][3] is ValueToken defV
                                          ? defV.Value
                                          : 0;

                        item.Size = val.Value;
                        if (defVal == 0)
                        {
                            item.IsEmpty = true;
                            emptyItems.Add( ( name.GetValue(), item ) );

                            Tokens.RemoveAt(i);
                            i--;
                            continue;
                        }
                        DataSection.AddRange(Enumerable.Repeat(defVal, (int)val.Value));
                    }
                    else
                    {
                        EventManager<ErrorEvent>.SendEvent(
                                                              new InvalidDataDefinitionEvent(name.GetValue())
                                                             );
                    }

                    DataSectionHeader[name.GetValue()] = item;
                    Tokens.RemoveAt(i);
                    i--;
                }
            }

            AppendFileSection();

            return emptyItems.ToArray();
        }

        private void CreateDataSection_Full()
        {
            for ( int i = 0; i < Tokens.Count; i++ )
            {
                if ( Tokens[i].Length >= 3 &&
                     Tokens[i][1] is WordToken name &&
                     Tokens[i][0] is WordToken word &&
                     word.GetValue() == ":data" )
                {
                    object[] linkerArgs = ParseLinkerArgs( Tokens[i].Skip( 3 ) );

                    AddressItem item = new AddressItem
                                                         {
                                                             Address = ( uint ) DataSection.Count,
                                                             LinkerArguments = linkerArgs,
                                                             IsEmpty = false
                                                         };

                    if ( Tokens[i][2] is StringToken str )
                    {
                        bool compress = linkerArgs.Contains( "string:packed" );
                        bool nullTerm = linkerArgs.Contains( "string:c-style" );
                        string data = str.GetContent();

                        if ( nullTerm )
                        {
                            data = data + '\0';
                        }

                        if ( compress )
                        {
                            List < byte > cData = Encoding.ASCII.GetBytes( data ).ToList();
                            cData.AddRange( Enumerable.Repeat( ( byte ) 0, cData.Count % sizeof( uint ) ) );

                            item.Size =(uint)(cData.Count / sizeof(uint));
                            for ( int j = 0; j < cData.Count / sizeof( uint ); j++ )
                            {
                                DataSection.Add( BitConverter.ToUInt32( cData.ToArray(), j ) );
                            }
                        }
                        else
                        {
                            item.Size=( uint ) data.Length;
                            DataSection.AddRange( data.Select( x => ( uint ) x ) );
                        }
                    }
                    else if ( Tokens[i][2] is ValueToken val )
                    {
                        uint defVal = Tokens[i].Length == 4 && Tokens[i][3] is ValueToken defV
                                          ? defV.Value
                                          : 0;

                        item.Size = val.Value;

                        DataSection.AddRange( Enumerable.Repeat( defVal, ( int ) val.Value ) );
                    }
                    else
                    {
                        EventManager < ErrorEvent >.SendEvent(
                                                              new InvalidDataDefinitionEvent( name.GetValue() )
                                                             );
                    }

                    DataSectionHeader[name.GetValue()] = item;
                    Tokens.RemoveAt( i );
                    i--;
                }
            }

            AppendFileSection();
        }

        private object[] ParseLinkerArgs( IEnumerable < AToken > tokens )
        {
            List < object > ret = new List < object >();

            foreach ( AToken aToken in tokens )
            {
                AToken item = aToken;

                if ( item is ValueToken vT )
                {
                    ret.Add( vT.Value );
                }
                else if ( item is StringToken lst )
                {
                    ret.Add( lst.GetContent() );
                }
                else
                {
                    ret.Add( item.GetValue() );
                }
            }

            return ret.ToArray();
        }

        private void ProcessFileReferences()
        {
            for ( int i = Tokens.Count - 1; i >= 0; i-- )
            {
                if ( Tokens[i][0] is WordToken word && word.GetValue() == ":include" )
                {
                    WordToken content = Tokens[i][1] as WordToken;

                    string cstr;

                    if ( content is StringToken st )
                    {
                        cstr = st.GetContent();
                    }
                    else
                    {
                        cstr = content.GetValue();
                    }

                    object[] linkerArgs = ParseLinkerArgs( Tokens[i].Skip( 2 ) );

                    string c = Path.GetFullPath(
                                                Path.Combine(
                                                             Path.GetDirectoryName( Reference.File ),
                                                             cstr
                                                            )
                                               );

                    FileReferences.Add( new FileReference( c, linkerArgs ) );
                    Tokens.RemoveAt( i );
                }
            }
        }

        private void ProcessLabels()
        {
            for ( int i = 0; i < Tokens.Count; i++ )
            {
                if ( Tokens[i].Length >= 1 && Tokens[i][0] is WordToken word && word.GetValue().StartsWith( "." ) )
                {
                    object[] linkerArgs = ParseLinkerArgs( Tokens[i].Skip( 1 ) );

                    Labels[Tokens[i][0].GetValue().Remove( 0, 1 )] = new AddressItem
                                                                     {
                                                                         Address =
                                                                             ( uint ) ( i *
                                                                                         CpuSettings.InstructionSize
                                                                                 ),
                                                                         LinkerArguments = linkerArgs,
                                                                     };

                    Tokens.RemoveAt( i );
                    i--;
                }
            }
        }

        private void ResolveConstantItems()
        {
            for ( int i = Tokens.Count - 1; i >= 0; i-- )
            {
                if ( Tokens[i][0] is WordToken word && word.GetValue() == ":const" )
                {
                    if ( Tokens[i].Length < 3 )
                    {
                        EventManager < ErrorEvent >.SendEvent(
                                                              new InvalidConstantDefinitionEvent(
                                                                   "Invalid Constant Statement"
                                                                  )
                                                             );
                    }

                    object[] linkerArgs = ParseLinkerArgs( Tokens[i].Skip( 3 ) );

                    Constants[Tokens[i][1].GetValue()] = new AddressItem
                                                         {
                                                             Address = ( Tokens[i][2] as ValueToken ).Value,
                                                             LinkerArguments = linkerArgs
                                                         };

                    Tokens.RemoveAt( i );
                }
            }

            ApplyToAllTokens( Tokens, Constants, new List < uint >() );
        }

        #endregion

    }

}
