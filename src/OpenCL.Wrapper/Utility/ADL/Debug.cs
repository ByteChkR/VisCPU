using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Utility.ADL.Configs;
using Utility.ADL.Streams;
using Utility.Collections;

/// <summary>
/// Namespace ADL is the "Root" namespace of ADL. It contains the Code needed to use ADL. But also in sub namespaces you will find other helpful tools.
/// </summary>
namespace Utility.ADL
{
    /// <summary>
    ///     Main Debug Class. No instanciation needed.
    /// </summary>
    public static class Debug
    {

        #region Private Variables

        internal static int DefaultSeverity { get; set; } = int.MaxValue;

        /// <summary>
        ///     Flag to check wether this is the first execution.
        /// </summary>
        private static bool FirstLog = true;


        private static readonly ADLLogger<LogType> InternalLogger =
            new ADLLogger<LogType>(InternalADLProjectDebugConfig.Settings, "Internal");

        private static readonly object PrefixLock = new object();


        /// <summary>
        ///     String Builder to assemble the log
        /// </summary>
        private static readonly StringBuilder StringBuilder = new StringBuilder();

        /// <summary>
        ///     List of LogStreams that are active
        /// </summary>
        private static readonly List<LogStream> Streams = new List<LogStream>();

        /// <summary>
        ///     Contains the flags that determine the way prefixes get looked up
        /// </summary>
        private static PrefixLookupSettings LookupMode = PrefixLookupSettings.AddPrefixIfAvailable;

        /// <summary>
        ///     The extracted flag if we should put tags at all
        /// </summary>
        private static bool AddPrefix;

        /// <summary>
        ///     The extracted flag if we should deconstruct the mask to find potential tags
        /// </summary>
        private static bool Deconstructtofind;

        /// <summary>
        ///     The extracted flag if we should end the lookup when one tag was found.(Does nothing if deconstruct flag is set to
        ///     false)
        /// </summary>
        private static bool Onlyone;

        /// <summary>
        ///     The extracted flag if ADL should bake prefixes on the fly to archieve better lookup performance.
        ///     This makes every new flag that is not prefixed a new prefix entry with the combined mask.
        /// </summary>
        private static bool BakePrefixes;

        #endregion

        #region Public Properties

        public static event Action<IProjectDebugConfig> OnConfigCreate;

        public static bool ShowSeverity { get; set; }

        internal static void ConfigCreated(IProjectDebugConfig config)
        {
            OnConfigCreate?.Invoke(config);
        }

        /// <summary>
        ///     The Encoding that is going to be used by all text in ADL.
        /// </summary>
        public static Encoding TextEncoding { get; set; } = Encoding.ASCII;

        /// <summary>
        ///     The format ADL uses to convert a Time to a string representation
        /// </summary>
        public static string TimeFormatString { get; set; }

        /// <summary>
        ///     Public property, used to disable ADl
        /// </summary>
        public static bool AdlEnabled { get; set; } = true;


        /// <summary>
        ///     The number of Streams that ADL writes to
        /// </summary>
        public static int LogStreamCount
        {
            get
            {
                lock (Streams)
                {
                    return Streams.Count;
                }
            }
        }

        public static PrefixLookupSettings PrefixLookupMode
        {
            get => LookupMode;
            set
            {
                AddPrefix =
                    BitMask.IsContainedInMask((int) value, (int) PrefixLookupSettings.AddPrefixIfAvailable, false);
                Deconstructtofind = BitMask.IsContainedInMask(
                                                              (int) value,
                                                              (int) PrefixLookupSettings.DeconstructMaskToFind,
                                                              false
                                                             );
                Onlyone = BitMask.IsContainedInMask((int) value, (int) PrefixLookupSettings.OnlyOnePrefix, false);
                LookupMode = value;
                BakePrefixes = BitMask.IsContainedInMask((int) value, (int) PrefixLookupSettings.BakePrefixes, false);
            }
        }

        #endregion

        #region Streams

        /// <summary>
        ///     Adds another stream to the debug logs.
        /// </summary>
        /// <param name="stream">The stream you want to add</param>
        public static void AddOutputStream(LogStream stream)
        {
            if (!initialized)
            {
                initialized = true;
            }


            if (stream == null)
            {
                InternalLogger.Log(LogType.Warning, "AddOutputStream(NULL): The Supplied stream is a nullpointer.", 1);
                return;
            }

            if (!AdlEnabled)
            {
                InternalLogger.Log(
                                   LogType.Warning,
                                   "AddOutputStream(" +
                                   stream +
                                   "): ADL is disabled, you are adding an Output Stream while ADL is disabled.",
                                   1
                                  );
            }

            bool contains = false;
            lock (Streams)
            {
                contains = Streams.Contains(stream);
            }


            if (contains)
            {
                InternalLogger.Log(
                                   LogType.Warning,
                                   "AddOutputStream(" + stream + "): Supplied stream is already in the list. Aborting!",
                                   1
                                  );
                return;
            }

            lock (Streams)
            {
                Streams.Add(stream);
            }
        }

        /// <summary>
        ///     Removes the specified Stream.
        /// </summary>
        /// <param name="stream">The stream you want to remove</param>
        /// ///
        /// <param name="closeStream">If streams should be closed upon removal from the system</param>
        public static void RemoveOutputStream(LogStream stream, bool closeStream = true)
        {
            bool contains = false;
            lock (Streams)
            {
                contains = Streams.Contains(stream);
            }

            if (!contains)
            {
                InternalLogger.Log(
                                   LogType.Warning,
                                   "RemoveOutputStream(" + stream + "): Supplied stream is not in the list. Aborting!",
                                   1
                                  );
                return;
            }

            if (!AdlEnabled)
            {
                InternalLogger.Log(
                                   LogType.Warning,
                                   "RemoveOutputStream(" +
                                   stream +
                                   "): ADL is disabled, you are removing an Output Stream while while ADL is disabled.",
                                   1
                                  );
            }

            lock (Streams)
            {
                Streams.Remove(stream);
            }

            if (closeStream)
            {
                stream.Close();
            }
        }

        /// <summary>
        ///     Removes all output streams from the list. Everything gets written to file.
        /// </summary>
        /// <param name="closeStream">If streams should be closed upon removal from the system</param>
        public static void RemoveAllOutputStreams(bool closeStream = true)
        {
            InternalLogger.Log(LogType.Log, "Debug Queue Emptied", 1);
            lock (Streams)
            {
                if (closeStream)
                {
                    foreach (LogStream ls in Streams)
                    {
                        ls.Close();
                    }
                }

                Streams.Clear();
            }
        }

        #endregion

        #region Prefixes

        /// <summary>
        ///     Adds a prefix for the specified level
        /// </summary>
        /// <param name="mask">flag combination</param>
        /// <param name="prefix">desired prefix</param>
        internal static void AddPrefixForMask(Dictionary<int, string> prefixes, BitMask mask, string prefix)
        {
            if (!AdlEnabled)
            {
                InternalLogger.Log(
                                   LogType.Warning,
                                   "AddPrefixForMask(" +
                                   mask +
                                   "): ADL is disabled, you are adding a prefix for a mask while ADL is disabled.",
                                   1
                                  );
            }


            if (!BitMask.IsUniqueMask(mask))
            {
                InternalLogger.Log(
                                   LogType.Warning,
                                   "AddPrefixForMask(" +
                                   mask +
                                   "): Adding Prefix: " +
                                   prefix +
                                   " for mask: " +
                                   mask +
                                   ". Mask is not unique.",
                                   1
                                  );
            }

            lock (PrefixLock)
            {
                if (prefixes.ContainsKey(mask))
                {
                    prefixes[mask] = prefix;
                }
                else
                {
                    prefixes.Add(mask, prefix);
                }
            }
        }

        /// <summary>
        ///     Removes Prefix from prefix lookup table
        /// </summary>
        /// <param name="mask"></param>
        internal static void RemovePrefixForMask(Dictionary<int, string> prefixes, BitMask mask)
        {
            if (!AdlEnabled)
            {
                InternalLogger.Log(
                                   LogType.Warning,
                                   "RemovePrefixForMask(" +
                                   mask +
                                   "): ADL is disabled, you are removing a prefix for a mask while ADL is disabled.",
                                   1
                                  );
            }

            lock (PrefixLock)
            {
                if (!prefixes.ContainsKey(mask))
                {
                    return;
                }

                prefixes.Remove(mask);
            }
        }

        /// <summary>
        ///     Clears all Prefixes
        /// </summary>
        internal static void RemoveAllPrefixes(Dictionary<int, string> prefixes)
        {
            lock (PrefixLock)
            {
                prefixes.Clear();
            }
        }

        /// <summary>
        ///     Sets all Prefixes from the list from low to high.
        ///     You can not specify the level, because it will fill the prefixes by power of 2. So prefixes[0] = level1 and
        ///     prefixes[2] = level4 and so on
        /// </summary>
        /// <param name="prefixes">List of prefixes</param>
        internal static void SetAllPrefixes(Dictionary<int, string> prefixes, params string[] prefixNames)
        {
            if (!AdlEnabled)
            {
                string info = "";
                prefixes.ToList().ForEach(x => info += x + ", ");
                InternalLogger.Log(
                                   LogType.Warning,
                                   "SetAllPrefixes(" +
                                   info +
                                   "): ADL is disabled, you are removing a prefix for a mask while ADL is disabled.",
                                   1
                                  );
            }

            RemoveAllPrefixes(prefixes);

            for (int i = 0; i < prefixNames.Length; i++)
            {
                AddPrefixForMask(prefixes, MathF.IntPow(2, i), prefixNames[i]);
            }
        }

        /// <summary>
        ///     Gets all Tags with corresponding masks.
        /// </summary>
        /// <returns></returns>
        internal static Dictionary<int, string> GetAllPrefixes(Dictionary<int, string> prefixes)
        {
            if (!AdlEnabled)
            {
                InternalLogger.Log(
                                   LogType.Warning,
                                   "GetAllPrefixes(): ADL is disabled, you are getting all prefixes while ADL is disabled.",
                                   1
                                  );
            }

            lock (prefixes)
            {
                return new Dictionary<int, string>(prefixes);
            }
        }

        #endregion

        #region Logging

        /// <summary>
        ///     Fire Log Messsage with desired level(flag) and message
        /// </summary>
        /// <param name="mask">the flag</param>
        /// <param name="message">the message</param>
        internal static void Log(ADLLogger logger, int mask, string message)
        {
            if (!AdlEnabled)
            {
                return;
            }

            if (FirstLog)
            {
                FirstLog = false;
            }

            string messg = message + '\n';
            string mesg = $"[{logger.GetMaskPrefix(mask)}]{messg}";

            lock (Streams)
            {
                for (int i = Streams.Count - 1; i >= 0; i--)
                {
                    if (Streams[i].IsClosed)
                    {
                        Streams.RemoveAt(i);
                    }
                }

                foreach (LogStream logs in Streams)
                {
                    logs.Write(logs.OverrideChannelTag ? new Log(mask, messg) : new Log(mask, mesg));
                }
            }
        }


        /// <summary>
        ///     Gets the Mask of the Specified Prefix
        /// </summary>
        /// <param name="prefix">Prefix</param>
        /// <param name="mask">Mask returned by the function</param>
        /// <returns>True if mask is found in Dictionary</returns>
        internal static bool GetPrefixMask(Dictionary<int, string> prefixes, string prefix, out BitMask mask)
        {
            mask = 0;
            Dictionary<int, string> prefx;
            lock (PrefixLock)
            {
                prefx = new Dictionary<int, string>(prefixes);
            }

            if (!prefx.ContainsValue(prefix))
            {
                return false;
            }

            foreach (KeyValuePair<int, string> kvp in prefx)
            {
                if (prefix == kvp.Value)
                {
                    mask = kvp.Key;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Returns the concatenated string of all the Prefixes that are fallin in that mask.
        /// </summary>
        /// <param name="mask"></param>
        /// <returns>All Prefixes for specified mask</returns>
        internal static string GetMaskPrefix(Dictionary<int, string> prefixes, BitMask mask)
        {
            if (!AddPrefix)
            {
                return "";
            }

            StringBuilder.Length = 0;
            lock (prefixes)
            {
                if (prefixes.ContainsKey(mask))
                {
                    //We happen to have a custom prefix for the level
                    StringBuilder.Append(prefixes[mask]);
                }
                else if (Deconstructtofind) //We have no Prefix specified for this particular level
                {
                    List<int>
                        flags = BitMask.GetUniqueMasksSet(mask); //Lets try to split all the flags into unique ones
                    foreach (int t in flags)
                    {
                        if (prefixes.ContainsKey(t))
                        {
                            StringBuilder.Insert(0, prefixes[t]);

                            if (Onlyone)
                            {
                                break;
                            }
                        }
                        else //If still not in prefix lookup table, better have a prefix than having just plain text.
                        {
                            StringBuilder.Insert(0, "[Log Mask:" + t + "]");

                            if (Onlyone)
                            {
                                break;
                            }
                        }
                    }

                    if (!BakePrefixes)
                    {
                        return StringBuilder.ToString();
                    }

                    lock (PrefixLock)
                    {
                        prefixes.Add(
                                     mask,
                                     StringBuilder.ToString()
                                    ); //Create a "custom prefix" with the constructed mask.
                    }
                }
            }

            return StringBuilder.ToString();
        }

        #endregion

        #region Config

        private static bool initialized;

        public static void DefaultInitialization()
        {
            if (initialized)
            {
                return;
            }

            LoadConfig((ADLConfig) new ADLConfig().GetStandard());
            LogTextStream lts = new LogTextStream(Console.OpenStandardOutput());
            AddOutputStream(lts);
        }

        /// <summary>
        ///     Loads a supplied ADLConfig.
        /// </summary>
        /// <param name="config">Config to load</param>
        public static void LoadConfig(ADLConfig config)
        {
            AdlEnabled = config.AdlEnabled;
            TimeFormatString = config.TimeFormatString;
            PrefixLookupMode = config.PrefixLookupMode;
        }


        public static void LoadConfig()
        {
            LoadConfig("adl_config.xml");
        }

        /// <summary>
        ///     Loads the ADL Config from the file at the supplied path
        /// </summary>
        /// <param name="path">file path</param>
        public static void LoadConfig(string path)
        {
            ADLConfig config = ConfigManager.ReadFromFile<ADLConfig>(path);
            LoadConfig(config);
        }

        /// <summary>
        ///     Saves the configuration to the given file path
        /// </summary>
        /// <param name="config">config to save</param>
        /// <param name="path">file path</param>
        public static void SaveConfig(ADLConfig config, string path)
        {
            ConfigManager.SaveToFile(path, config);
        }

        public static void SaveConfig(ADLConfig config)
        {
            SaveConfig(config, "adl_config.xml");
        }

        public static void SaveConfig()
        {
            SaveConfig("adl_config.xml");
        }

        /// <summary>
        ///     Saves the current configuration of ADL to the given file path
        /// </summary>
        /// <param name="path">File path.</param>
        public static void SaveConfig(string path)
        {
            ADLConfig config = ConfigManager.GetDefault<ADLConfig>();
            config.AdlEnabled = AdlEnabled;
            config.TimeFormatString = TimeFormatString;
            config.PrefixLookupMode = PrefixLookupMode;
            SaveConfig(config, path);
        }

        #endregion

    }
}