using System;


namespace Utility.ExtPP.API.Exceptions
{
    /// <summary>
    ///     Exception that occurs when the Text Processor encounters an error.
    /// </summary>
    public class TextProcessingException : Exception
    {

        public TextProcessingException(string errorMessage, ApplicationException inner) : base(errorMessage, inner)
        {
        }

    }
}