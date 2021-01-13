using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.Compiler.Compiler.Events;
using VisCPU.Compiler.Parser;
using VisCPU.Compiler.Parser.Tokens;
using VisCPU.Utility;
using VisCPU.Utility.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.Logging;
using VisCPU.Utility.SharedBase;

namespace VisCPU.Compiler.Compiler
{

    public class FileCompilation : VisBase
    {

        public readonly Dictionary < string, AddressItem > Constants = new Dictionary < string, AddressItem >();
        public readonly List < uint > DataSection = new List < uint >();
        public readonly Dictionary < string, AddressItem > DataSectionHeader = new Dictionary < string, AddressItem >();
        public readonly List < FileReference > FileReferences = new List < FileReference >();
        public readonly Dictionary < string, AddressItem > Labels = new Dictionary < string, AddressItem >();
        public readonly LinkerInfo LinkerInfo;
        public readonly FileReference Reference;

        public readonly string Source;
        public readonly List < AToken[] > Tokens;

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
        }

        public FileCompilation( FileReference file )
        {
            Reference = file;
            Source = File.ReadAllText( Reference.File );
            Tokens = Tokenizer.Tokenize( Source );
            ResolveConstantItems();
            ProcessFileReferences();
            CreateDataSection();
            ProcessLabels();
        }

        public static void ApplyToAllTokens( List < AToken[] > tokens, IDictionary < string, AddressItem > header )
        {
            for ( int i = 0; i < tokens.Count; i++ )
            {
                for ( int j = 1; j < tokens[i].Length; j++ )
                {
                    string w = tokens[i][j].GetValue();

                    if ( header.ContainsKey( w ) )
                    {
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

                    DataSectionHeader[name.GetValue()] = new AddressItem
                                                         {
                                                             Address = ( uint ) DataSection.Count,
                                                             LinkerArguments = linkerArgs
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

                    uint[] data = File.ReadAllBytes( file ).ToUInt();

                    DataSection.AddRange( data );

                    DataSectionHeader[name.GetValue() + "_LEN"] = new AddressItem
                                                                  {
                                                                      Address = ( uint ) data.Length,
                                                                      LinkerArguments =
                                                                          new[]
                                                                          {
                                                                              "linker:autogen",
                                                                              "linker:file_length",
                                                                              "linker:hide"
                                                                          }
                                                                  };

                    Tokens.RemoveAt( i );
                    i--;
                }
            }
        }

        private void CreateDataSection()
        {
            for ( int i = 0; i < Tokens.Count; i++ )
            {
                if ( Tokens[i].Length >= 3 &&
                     Tokens[i][1] is WordToken name &&
                     Tokens[i][0] is WordToken word &&
                     word.GetValue() == ":data" )
                {
                    object[] linkerArgs = ParseLinkerArgs( Tokens[i].Skip( 3 ) );

                    DataSectionHeader[name.GetValue()] = new AddressItem
                                                         {
                                                             Address = ( uint ) DataSection.Count,
                                                             LinkerArguments = linkerArgs
                                                         };

                    if ( Tokens[i][2] is StringToken str )
                    {
                        DataSection.AddRange( str.GetContent().ToCharArray().Select( x => ( uint ) x ) );
                    }
                    else if ( Tokens[i][2] is ValueToken val )
                    {
                        uint defVal = Tokens[i].Length == 4 && Tokens[i][3] is ValueToken defV
                                          ? defV.Value
                                          : 0;

                        DataSection.AddRange( Enumerable.Repeat( defVal, ( int ) val.Value ) );
                    }
                    else
                    {
                        EventManager < ErrorEvent >.SendEvent(
                                                              new InvalidDataDefinitionEvent( name.GetValue() )
                                                             );
                    }

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
                else if (item is StringToken lst)
                {
                    ret.Add(lst.GetContent());
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

                    Log( "Including file: {0}", cstr);
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
                                                                                         CPUSettings.INSTRUCTION_SIZE ),
                                                                         LinkerArguments = linkerArgs
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

            ApplyToAllTokens( Tokens, Constants );
        }

        #endregion

    }

}
