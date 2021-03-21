using System;
using System.Collections.Generic;
using System.Linq;

using Utility.FastString;

namespace OpenCL.Wrapper
{
    public struct CLProgramBuildResult
    {

        public readonly string TargetFile;

        public bool Success => BuildErrors.Count == 0;

        public readonly List<CLProgramBuildError> BuildErrors;
        public string Source;

        public CLProgramBuildResult(string targetFile, string source, List<CLProgramBuildError> errors)
        {
            Source = source;
            TargetFile = targetFile;
            BuildErrors = errors;
        }

        public AggregateException GetAggregateException()
        {
            return new AggregateException(BuildErrors.Select(x => x.Exception));
        }

        public static implicit operator bool(CLProgramBuildResult result)
        {
            return result.Success;
        }

        public override string ToString()
        {
            return $"{TargetFile}: \n\t{BuildErrors.Select(x => x.ToString()).Unpack("\n\t")}";
        }

    }
}