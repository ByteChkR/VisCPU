using System.Collections.Generic;
using System.Linq;

namespace VisCPU.HL.Importer
{

    public static class ImporterSystem
    {

        private static List < IImporter > s_Importer = new List < IImporter >();

        #region Public

        public static void Add( params IImporter[] imp )
        {
            s_Importer.AddRange( imp );
        }

        public static IImporter Get( string input )
        {
            return s_Importer.FirstOrDefault( x => x.CanImport( input ) );
        }

        #endregion

    }

}
