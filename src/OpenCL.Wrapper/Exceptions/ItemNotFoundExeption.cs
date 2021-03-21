using System;


namespace OpenCL.Wrapper.Exceptions
{
    /// <summary>
    ///     Occurs when a file is not found.
    /// </summary>
    public class ItemNotFoundExeption : Exception
    {

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="itemType">Type of the Item that has not been found</param>
        /// <param name="desc">Description what caused the crash</param>
        /// <param name="inner">Inner Exception</param>
        public ItemNotFoundExeption(string itemType, string desc, Exception inner) : base(
             $"The Item {itemType} could not be Found.\n Description: {desc}",
             inner
            )
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="itemType">Type of the Item that has not been found</param>
        /// <param name="desc">Description what caused the crash</param>
        public ItemNotFoundExeption(string itemType, string desc) : this(itemType, desc, null)
        {
        }

    }
}