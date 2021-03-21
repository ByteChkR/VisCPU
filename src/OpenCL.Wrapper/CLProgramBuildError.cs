using System;

namespace OpenCL.Wrapper
{
    public struct CLProgramBuildError
    {

        public readonly ErrorType Error;
        public readonly Exception Exception;

        public string Message =>
            Exception.InnerException != null ? Exception.InnerException.Message : Exception.Message;

        public CLProgramBuildError(ErrorType error, Exception exception)
        {
            Error = error;
            Exception = exception;
        }

        public override string ToString()
        {
            return $"[{Error}] {Exception.GetType().Name} {Message}";
        }

    }
}