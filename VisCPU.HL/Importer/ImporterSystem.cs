using System.Collections.Generic;
using System.Linq;

namespace VisCPU.HL.Importer
{

    public static class ImporterSystem
    {

        private static List < IImporter > importer = new List < IImporter >();

        #region Public

        public static void Add( params IImporter[] imp )
        {
            importer.AddRange( imp );
        }

        public static IImporter Get( string input )
        {
            return importer.FirstOrDefault( x => x.CanImport( input ) );
        }

        #endregion

    }

}
