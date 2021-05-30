using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using VisCPU.Utility.ArgumentParser;
using VisCPU.Utility.Extensions;
using VisCPU.Utility.Logging;

namespace VisCPU.HL.TextLoader
{

    public abstract class TextImporter
    {

        private static readonly List < TextImporter > s_ImporterList;

        private static readonly string s_DefaultImporter = "VPPImporter";
        private static string s_Importer = "PlainTextImporter";
        private static readonly string s_ImporterFile;

        public static IEnumerable < (string, string) > ImporterArgs { get; private set; } = new (string, string)[0];

        public abstract string Name { get; }

        #region Public

        public static string ImportFile( string text, string rootDir )
        {
            return ImportFile( s_Importer, text, rootDir );
        }

        public static string ImportFile( string importerName, string text, string rootDir )
        {
            TextImporter s = s_ImporterList.FirstOrDefault( x => x.Name == importerName );

            if ( s == null )
            {
                Logger.LogMessage( LoggerSystems.HlCompiler, "Can not Find importer: {0}", importerName );

                throw new Exception();
            }

            return s.Import( text, rootDir );
        }

        public static void ParseImporterArgs( string[] args )
        {
            TextImporterSettings s = new TextImporterSettings();
            ArgumentSyntaxParser.Parse( args, s );
            SetImporterArgs( s.DefinedSymbols );
        }

        public static void ParseImporterArgs()
        {
            TextImporterSettings s = new TextImporterSettings();
            ArgumentSyntaxParser.Parse( Environment.GetCommandLineArgs(), s );
            SetImporterArgs( s.DefinedSymbols );
        }

        public static void SetDefaultImporter()
        {
            s_Importer = s_DefaultImporter;
        }

        public static void SetImporter( string importer )
        {
            s_Importer = importer;
            File.WriteAllText( s_ImporterFile, s_Importer );
        }

        public static void SetImporterArgs( (string, string)[] args )
        {
            ImporterArgs = args;
        }

        public abstract string Import( string text, string rootDir );

        #endregion

        #region Private

        static TextImporter()
        {
            string dir = CpuSettings.CpuExtensionsCategory.
                                     AddCategory( "text-importer" ).
                                     GetCategoryDirectory();

            s_ImporterList = ExtensionLoader.LoadFrom < TextImporter >(
                                                                       dir,
                                                                       true
                                                                      ).
                                             ToList();

            if ( s_ImporterList.All( x => !( x is PlainTextImporter ) ) )
            {
                s_ImporterList.Add( new PlainTextImporter() );
            }

            s_ImporterFile = Path.Combine(
                                          dir,
                                          "importer.txt"
                                         );

            string importer = s_DefaultImporter;

            if ( File.Exists( s_ImporterFile ) )
            {
                importer = File.ReadAllText( s_ImporterFile ).Trim();
            }

            s_Importer = importer;
        }

        #endregion

    }

}
