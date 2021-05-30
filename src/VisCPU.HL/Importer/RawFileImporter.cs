using System.IO;

using VisCPU.HL.DataTypes;
using VisCPU.HL.Importer.Events;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Importer
{

    public class RawFileImporter : AImporter, IDataImporter
    {

        private const string RAW_FILE_IMPORTER_TAG = "raw-file";

        public override bool CanImport(string input)
        {
            return input.StartsWith(RAW_FILE_IMPORTER_TAG);
        }

        public IExternalData[] ProcessImport(HlCompilation compilation, string input)
        {
            int tagLen = RAW_FILE_IMPORTER_TAG.Length + 1;

            if (input.Length < tagLen)
            {
                EventManager<ErrorEvent>.SendEvent(new InvalidVasmBridgeArgumentsEvent(input));

                return null;
            }

            string[] cmd = input.Remove(0, tagLen).Split(' ');

            string name = cmd[0];
            string nameL = name + "_LEN";
            string filePath = Path.GetFullPath(cmd[1]);
            compilation.EmitterResult.Store($":file {name} \"{filePath}\"");

            return new IExternalData[]
                   {
                       new VariableData(
                                        name,
                                        name,
                                        1,
                                        compilation.TypeSystem.GetType(
                                                                       compilation.Root,
                                                                       HLBaseTypeNames.s_UintTypeName
                                                                      ),
                                        VariableDataEmitFlags.None
                                       ),
                       new VariableData(
                                        nameL,
                                        nameL,
                                        1,
                                        compilation.TypeSystem.GetType(
                                                                       compilation.Root,
                                                                       HLBaseTypeNames.s_UintTypeName
                                                                      ),
                                        VariableDataEmitFlags.None
                                       ),
                   };
        }

    }

}