using System.Collections.Generic;
using System.Linq;

using VisCPU.Utility.SharedBase;

namespace VisCPU.Compiler.Linking
{

    public class LinkerResult
    {

        public Dictionary < string, AddressItem > Constants { get; }

        public List < uint > DataSection { get; }

        public Dictionary < string, AddressItem > DataSectionHeader { get; }

        public Dictionary < (int, int), Dictionary < string, AddressItem > > HiddenConstantItems { get; }

        public Dictionary < (int, int), Dictionary < string, AddressItem > > HiddenDataSectionItems { get; }

        public Dictionary < (int, int), Dictionary < string, AddressItem > > HiddenLabelItems { get; }

        public Dictionary < string, AddressItem > Labels { get; }

        public List < AToken[] > LinkedBinary { get; }

        public LinkerTarget[] Targets { get; }

        #region Public

        public LinkerResult(
            LinkerTarget[] targets,
            Dictionary < string, AddressItem > constants,
            Dictionary < string, AddressItem > labels,
            Dictionary < string, AddressItem > dataSectionHeader,
            List < AToken[] > linkedBinary,
            uint[] dataSection )
        {
            Targets = targets;
            Constants = constants;
            Labels = labels;
            DataSectionHeader = dataSectionHeader;
            DataSection = dataSection.ToList();
            LinkedBinary = linkedBinary;
            HiddenLabelItems = new Dictionary < (int, int), Dictionary < string, AddressItem > >();
            HiddenConstantItems = new Dictionary < (int, int), Dictionary < string, AddressItem > >();
            HiddenDataSectionItems = new Dictionary < (int, int), Dictionary < string, AddressItem > >();
        }

        public void ApplyDataOffset( int off )
        {
            Dictionary < string, AddressItem > items = DataSectionHeader.ToDictionary( x => x.Key, x => x.Value );
            DataSectionHeader.Clear();

            foreach ( KeyValuePair < string, AddressItem > keyValuePair in items )
            {
                DataSectionHeader[keyValuePair.Key] = new AddressItem
                                                      {
                                                          Address = ( uint ) ( keyValuePair.Value.Address + off ),
                                                          LinkerArguments = keyValuePair.Value.LinkerArguments
                                                      };
            }
        }

        #endregion

    }

}
