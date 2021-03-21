using System;
using System.Text;
using System.Xml.Serialization;

namespace Utility.ADL.Configs
{
    /// <summary>
    ///     Contains the Configurations of the main ADL.Debug class.
    /// </summary>
    [Serializable]
    public class ADLConfig : AbstractADLConfig
    {

        /// <summary>
        ///     Is ADL enabled when this config is loaded?
        /// </summary>
        public bool AdlEnabled { get; set; }


        /// <summary>
        ///     Determines the Options on how much effort is put into finding the right tags
        /// </summary>
        public PrefixLookupSettings PrefixLookupMode { get; set; }


        [XmlIgnore]
        public Encoding TextEncoding { get; set; }


        /// <summary>
        ///     The format ADL uses to convert a Time to a string representation
        /// </summary>
        public string TimeFormatString { get; set; }


        /// <summary>
        ///     Standard Confuguration
        /// </summary>
        /// <returns>The standard configuration of ADL</returns>
        public override AbstractADLConfig GetStandard()
        {
            return new ADLConfig
                   {
                       AdlEnabled = true,
                       PrefixLookupMode = PrefixLookupSettings.AddPrefixIfAvailable |
                                          PrefixLookupSettings.DeconstructMaskToFind,
                       TextEncoding = Encoding.ASCII,
                       TimeFormatString = "MM-dd-yyyy-H-mm-ss"
                   };
        }

        #region Lookup Presets

        public static PrefixLookupSettings LowestPerformance =>
            PrefixLookupSettings.AddPrefixIfAvailable |
            PrefixLookupSettings.DeconstructMaskToFind;

        public static PrefixLookupSettings LowPerformance =>
            PrefixLookupSettings.AddPrefixIfAvailable |
            PrefixLookupSettings.DeconstructMaskToFind |
            PrefixLookupSettings.OnlyOnePrefix;

        public static PrefixLookupSettings MediumPerformance =>
            PrefixLookupSettings.AddPrefixIfAvailable |
            PrefixLookupSettings.DeconstructMaskToFind |
            PrefixLookupSettings.BakePrefixes;

        public static PrefixLookupSettings HighPerformance => PrefixLookupSettings.AddPrefixIfAvailable;

        public static PrefixLookupSettings HighestPerformance => PrefixLookupSettings.NoPrefix;

        #endregion

    }
}