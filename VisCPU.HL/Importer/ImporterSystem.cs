using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;

namespace VisCPU.HL.Importer
{

    public static class ImporterSystem
    {

        private static List<IImporter> importer = new List<IImporter>();

        public static void Add(params IImporter[] imp) => importer.AddRange(imp);
        public static IImporter Get(string input) => importer.FirstOrDefault(x => x.CanImport(input));

    }

}
