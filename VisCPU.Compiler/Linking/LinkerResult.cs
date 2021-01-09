using System.Collections.Generic;
using System.Linq;

using VisCPU.Utility.SharedBase;

namespace VisCPU.Compiler.Linking
{

    public class LinkerResult
    {

        public readonly Dictionary < string, AddressItem > Constants;
        public readonly List < uint > DataSection;
        public readonly Dictionary < string, AddressItem > DataSectionHeader;
        public readonly Dictionary < (int, int), Dictionary < string, AddressItem > > HiddenConstantItems;
        public readonly Dictionary < (int, int), Dictionary < string, AddressItem > > HiddenDataSectionItems;

        public readonly Dictionary < (int, int), Dictionary < string, AddressItem > > HiddenLabelItems;
        public readonly Dictionary < string, AddressItem > Labels;

        public readonly List < AToken[] > LinkedBinary;
        public readonly LinkerTarget[] Targets;

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

        #endregion

    }

}
