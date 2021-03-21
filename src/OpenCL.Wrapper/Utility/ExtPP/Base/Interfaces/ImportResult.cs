using System.Collections.Generic;

namespace Utility.ExtPP.Base.Interfaces
{
    /// <summary>
    ///     A data object that is used to hold information about the sourcescript file and key
    /// </summary>
    public class ImportResult
    {

        /// <summary>
        ///     The underlying data structure that is used to hold the custom data
        /// </summary>
        private readonly Dictionary<string, object> data = new Dictionary<string, object>();

        /// <summary>
        ///     A flag indicating the success state of the Import Operation
        /// </summary>
        private bool result;

        /// <summary>
        ///     Sets a key value pair.
        /// </summary>
        /// <param name="key">The key to be set</param>
        /// <param name="value">the value that will be set</param>
        public void SetValue(string key, object value)
        {
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
            else
            {
                data.Add(key, value);
            }
        }

        /// <summary>
        ///     Returns a value from the custom data
        /// </summary>
        /// <param name="key">the key of the value</param>
        /// <returns>the object</returns>
        public object GetValue(string key)
        {
            return data[key];
        }

        /// <summary>
        ///     Returns a value from the custom data
        /// </summary>
        /// <param name="key">the key of the value</param>
        /// <returns>the object cast to string</returns>
        public string GetString(string key)
        {
            return (string) data[key];
        }

        /// <summary>
        ///     Sets the result of the operation
        /// </summary>
        /// <param name="result">the result</param>
        public void SetResult(bool result)
        {
            this.result = result;
        }

        /// <summary>
        ///     Checks the Custom data for a specific key
        /// </summary>
        /// <param name="key">The key to be checked</param>
        /// <returns>True if the key is contained in the data</returns>
        public bool ContainsKey(string key)
        {
            return data.ContainsKey(key);
        }

        /// <summary>
        ///     Removes an entry from the data
        /// </summary>
        /// <param name="key">the key to be removed</param>
        public void RemoveEntry(string key)
        {
            if (data.ContainsKey(key))
            {
                data.Remove(key);
            }
        }

        /// <summary>
        ///     Implicit operator to convert to bool
        /// </summary>
        /// <param name="obj">The object to be converted</param>
        public static implicit operator bool(ImportResult obj)
        {
            return obj.result;
        }

    }
}