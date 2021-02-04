using System.Collections.Generic;
using System.Linq;

using VisCPU.HL.DataTypes;
using VisCPU.HL.Importer.Events;
using VisCPU.HL.TypeSystem;
using VisCPU.Utility.EventSystem;
using VisCPU.Utility.EventSystem.Events;
using VisCPU.Utility.SharedBase;

namespace VisCPU.HL.Importer
{

    public class LinkerImporter : AImporter, IDataImporter
    {

        #region Public

        public override bool CanImport( string input )
        {
            return input.StartsWith( "link" );
        }

        public IExternalData[] ProcessImport(HlCompilation compilation, string input )
        {
            int tagLen = "link".Length + 1;

            if ( input.Length < tagLen )
            {
                EventManager < ErrorEvent >.SendEvent( new InvalidLinkImporterArgumentsEvent( input ) );

                return new IExternalData[0];
            }

            string cmd = input.Remove( 0, tagLen );

            LinkerInfo info = LinkerInfo.Load( cmd );

            foreach ( KeyValuePair < string, AddressItem > label in info.Labels )
            {
                if ( label.Key.StartsWith( "FUN_" ) )
                {
                    string[] lblParts = label.Key.Remove(0, "FUN_".Length).Split( '_' );
                    HlTypeDefinition tdef= compilation.TypeSystem.GetOrAdd(lblParts[0], true);
                    tdef.AddMember( new HlExternalFunctionDefinition( lblParts[1], label.Key ) );
                }
            }

            return info.Labels.
                        Select(
                               x => ( IExternalData ) new LinkedData(
                                                                     x.Key,
                                                                     x.Value,
                                                                     ExternalDataType.Function
                                                                    )
                              ).
                        Concat(
                               info.DataSectionHeader.Select(
                                                             x => ( IExternalData ) new LinkedData(
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
