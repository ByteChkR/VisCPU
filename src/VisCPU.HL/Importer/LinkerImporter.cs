using System.Collections.Generic;
using System.Linq;

using VisCPU.HL.DataTypes;
using VisCPU.HL.Importer.Events;
using VisCPU.HL.Parser;
using VisCPU.HL.Parser.Tokens;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Importer
{

    public class LinkerImporter : AImporter, IDataImporter
    {

        #region Public

        public override bool CanImport(string input)
        {
            return input.StartsWith("link");
        }

        public IExternalData[] ProcessImport(HlCompilation compilation, string input)
        {
            int tagLen = "link".Length + 1;

            if (input.Length < tagLen)
            {
                EventManager<ErrorEvent>.SendEvent(new InvalidLinkImporterArgumentsEvent(input));

                return new IExternalData[0];
            }

            string cmd = input.Remove(0, tagLen);

            LinkerInfo info = LinkerInfo.Load(cmd);

            foreach (KeyValuePair<string, AddressItem> label in info.Labels)
            {
                if (label.Key.StartsWith("SFUN_"))
                {
                    string raw = label.Key.Remove(0, "SFUN_".Length);
                    int idx = raw.IndexOf("_");
                    string name = raw.Substring(0, idx);
                    HlTypeDefinition tdef = compilation.TypeSystem.HasType(compilation.Root, name) ? compilation.TypeSystem.GetType(compilation.Root, name) : compilation.TypeSystem.CreateEmptyType(compilation.Root, name, true, false);

                    tdef.AddMember(
                                   new HlExternalFunctionDefinition(compilation.TypeSystem,
                                                                    compilation.Root,
                                                                    raw.Remove(0, idx + 1),
                                                                    label.Key,
                                                                    new List<IHlToken>
                                                                    {
                                                                        new HlTextToken(
                                                                             HlTokenType.OpPublicMod,
                                                                             "public",
                                                                             0
                                                                            ),
                                                                        new HlTextToken(
                                                                             HlTokenType.OpStaticMod,
                                                                             "static",
                                                                             0
                                                                            )
                                                                    }
                                                                   )
                                  );
                }
                else if (label.Key.StartsWith("ADFUN_"))
                {
                    string raw = label.Key.Remove(0, "ADFUN_".Length);
                    int idx = raw.IndexOf("_");
                    string name = raw.Substring(0, idx);
                    HlTypeDefinition tdef = compilation.TypeSystem.HasType(compilation.Root, name) ? compilation.TypeSystem.GetType(compilation.Root, name) : compilation.TypeSystem.CreateEmptyType(compilation.Root, name, true, false);

                    tdef.AddMember(
                                   new HlExternalFunctionDefinition(compilation.TypeSystem,
                                                                    compilation.Root,
                                                                    raw.Remove(0, idx + 1),
                                                                    label.Key,
                                                                    new List<IHlToken>
                                                                    {
                                                                        new HlTextToken(
                                                                             HlTokenType.OpPublicMod,
                                                                             "public",
                                                                             0
                                                                            ),
                                                                        new HlTextToken(
                                                                             HlTokenType.OpAbstractMod,
                                                                             "abstract",
                                                                             0
                                                                            ),
                                                                    }
                                                                   )
                                  );
                }
                else if (label.Key.StartsWith("DFUN_"))
                {
                    string raw = label.Key.Remove(0, "DFUN_".Length);
                    int idx = raw.IndexOf("_");
                    string name = raw.Substring(0, idx);
                    HlTypeDefinition tdef = compilation.TypeSystem.HasType(compilation.Root, name) ? compilation.TypeSystem.GetType(compilation.Root, name) : compilation.TypeSystem.CreateEmptyType(compilation.Root, name, true, false);

                    tdef.AddMember(
                                   new HlExternalFunctionDefinition(compilation.TypeSystem,
                                                                    compilation.Root,
                                                                    raw.Remove(0, idx + 1),
                                                                    label.Key,
                                                                    new List<IHlToken>
                                                                    {
                                                                        new HlTextToken(
                                                                             HlTokenType.OpPublicMod,
                                                                             "public",
                                                                             0
                                                                            ),
                                                                    }
                                                                   )
                                  );
                }
                else if (label.Key.StartsWith("VDFUN_"))
                {
                    string raw = label.Key.Remove(0, "VDFUN_".Length);
                    int idx = raw.IndexOf("_");
                    string name = raw.Substring(0, idx);
                    HlTypeDefinition tdef = compilation.TypeSystem.HasType(compilation.Root, name) ? compilation.TypeSystem.GetType(compilation.Root, name) : compilation.TypeSystem.CreateEmptyType(compilation.Root, name, true, false);


                    tdef.AddMember(
                                   new HlExternalFunctionDefinition(
                                                                    compilation.TypeSystem,
                                                                    compilation.Root,
                                                                    raw.Remove(0, idx + 1),
                                                                    label.Key,
                                                                    new List<IHlToken>
                                                                    {
                                                                            new HlTextToken(
                                                                                 HlTokenType.OpPublicMod,
                                                                                 "public",
                                                                                 0
                                                                                ),
                                                                            new HlTextToken(
                                                                                 HlTokenType.OpVirtualMod,
                                                                                 "virtual",
                                                                                 0
                                                                                )
                                                                    }
                                                                   )
                                  );

                }
            }

            return info.Labels.
                        Select(
                               x => (IExternalData)new LinkedData(
                                                                     x.Key,
                                                                     x.Value,
                                                                     ExternalDataType.Function
                                                                    )
                              ).
                        Concat(
                               info.DataSectionHeader.Select(
                                                             x => (IExternalData)new LinkedData(
                                                                  x.Key,
                                                                  x.Value,
                                                                  ExternalDataType.Variable
                                                                 )
                                                            )
                              ).
                        ToArray();
        }

        #endregion

    }

}
