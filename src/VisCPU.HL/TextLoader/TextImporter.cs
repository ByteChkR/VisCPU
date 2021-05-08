using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisCPU.Utility.Extensions;

namespace VisCPU.HL.TextLoader
{

    public abstract class TextImporter
    {
        private static readonly List < TextImporter > s_ImporterList;

        private static readonly string s_DefaultImporter = "VPPImporter";
        private static string s_Importer = "PlainTextImporter";
        private static readonly string s_ImporterFile;

        public abstract string Name { get; }

        #region Public

        public static string ImportFile( string text, string rootDir )
        {
            return ImportFile( s_Importer, text, rootDir );
        }

        public static string ImportFile( string importerName, string text, string rootDir )
        {
            string s = s_ImporterList.First( x => x.Name == importerName ).Import( text, rootDir );

            return s;
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
