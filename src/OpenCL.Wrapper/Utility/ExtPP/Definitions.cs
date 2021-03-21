using System.Collections.Generic;

using Utility.ADL;
using Utility.ExtPP.Base;
using Utility.ExtPP.Base.Interfaces;

namespace Utility.ExtPP
{
    /// <summary>
    ///     Contains the Values on what is Defined as a Variable when Processing the text
    /// </summary>
    public class Definitions : ALoggable<LogType>, IDefinitions
    {

        /// <summary>
        ///     Dictionary to keep track of what is defined and what is not
        /// </summary>
        private readonly Dictionary<string, bool> definitions;

        /// <summary>
        ///     Convenience Wrapper
        /// </summary>
        public Definitions() : this(new Dictionary<string, bool>())
        {
        }


        /// <summary>
        ///     Creates a Definitions Object with predefined definitions
        /// </summary>
        /// <param name="definitions">the predefined definitions</param>
        public Definitions(Dictionary<string, bool> definitions) : base(ExtPPDebugConfig.Settings, "Defs")
        {
            this.definitions = definitions;
        }

        /// <summary>
        ///     Set an array of definitions to true
        /// </summary>
        /// <param name="keys">The keys that will be set</param>
        public void Set(string[] keys)
        {
            foreach (string key in keys)
            {
                Set(key);
            }
        }

        /// <summary>
        ///     Set an array of definitions to false
        /// </summary>
        /// <param name="keys">The keys that will be unset</param>
        public void Unset(string[] keys)
        {
            foreach (string key in keys)
            {
                Unset(key);
            }
        }

        /// <summary>
        ///     Set a specific definition to true
        /// </summary>
        /// <param name="key">The key that will be set</param>
        public void Set(string key)
        {
            Change(key, true);
        }


        /// <summary>
        ///     Set a specific definition to false
        /// </summary>
        /// <param name="key">The key that will be unset</param>
        public void Unset(string key)
        {
            Change(key, false);
        }

        /// <summary>
        ///     Returns true if the definition is "set" and returns false if the definition is "unset"
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <returns>returns true when the key is set.</returns>
        public bool Check(string key)
        {
            return definitions.ContainsKey(key) && definitions[key];
        }


        /// <summary>
        ///     Change the definition state.
        /// </summary>
        /// <param name="key">definition name</param>
        /// <param name="state">The state of the key</param>
        private void Change(string key, bool state)
        {
            Logger.Log(LogType.Log, $"Setting Key: {key} to value: {state}", 6);
            if (definitions.ContainsKey(key))
            {
                definitions[key] = state;
            }
            else
            {
                definitions.Add(key, state);
            }
        }

    }
}