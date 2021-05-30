using System.IO;

using VisCPU.HL.DataTypes;
using VisCPU.HL.Importer.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Importer
{

    public class StringFileImporter : AImporter, IDataImporter
    {

        private const string STRING_FILE_IMPORTER_TAG = "string-file";

        public override bool CanImport(string input)
        {
            return input.StartsWith(STRING_FILE_IMPORTER_TAG);
        }

        public IExternalData[] ProcessImport(HlCompilation compilation, string input)
        {
            int tagLen = STRING_FILE_IMPORTER_TAG.Length + 1;

            if (input.Length < tagLen)
            {
                EventManager<ErrorEvent>.SendEvent(new InvalidVasmBridgeArgumentsEvent(input));

                return null;
            }

            string[] cmd = input.Remove(0, tagLen).Split(' ');

            string name = cmd[0];
            string filePath = Path.GetFullPath(cmd[1]);
            string text = File.ReadAllText( filePath ).Replace( "\r", "" );
            compilation.CreateVariable(
                                       name,
                                       text,
                                       compilation.TypeSystem.GetType(
                                                                      compilation.Root,
                                                                      HLBaseTypeNames.s_StringTypeName
                                                                     ),
                                       VariableDataEmitFlags.None
                                      );

            return new IExternalData[] { };
        }

    }

}