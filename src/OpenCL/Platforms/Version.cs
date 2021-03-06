#region Using Directives

using System;
using System.Text.RegularExpressions;

#endregion

namespace OpenCL.NET.Platforms
{

    /// <summary>
    ///     Represents the version of an OpenCL platform.
    /// </summary>
    public class Version
    {

        /// <summary>
        ///     Gets the original version string returned by the OpenCL platform, which is in the format
        ///     "OpenCL[space][major_version.minor_version][space][platform-specific information]".
        /// </summary>
        public string VersionString { get; }

        /// <summary>
        ///     Gets the major version of OpenCL that is supported by the OpenCL platform.
        /// </summary>
        public int MajorVersion { get; }

        /// <summary>
        ///     Gets the minor version of OpenCL that is supported by the OpenCL platform.
        /// </summary>
        public int MinorVersion { get; }

        /// <summary>
        ///     Gets the version information specific to the OpenCL platform.
        /// </summary>
        public string PlatformSpecificInformation { get; }

        #region Public

        /// <summary>
        ///     Initializes a new <see cref="Version" /> instance.
        /// </summary>
        /// <param name="versionString">
        ///     Gets the original version string returned by the OpenCL platform, which is in the
        ///     format"OpenCL[space][major_version.minor_version][space][platform-specific information]".
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If the version string could not be parsed, then an <see cref="ArgumentException" />
        ///     is thrown.
        /// </exception>
        public Version( string versionString )
        {
            // Saves the version string for later reference
            VersionString = versionString;

            // Creates a new regular expression, which is used to parse the version string
            Regex versionStringRegularExpression =
                new Regex(
                          "^OpenCL (?<MajorVersion>[0-9]+)\\.(?<MinorVersion>[0-9]+) (?<PlatformSpecificInformation>.*)$"
                         );

            // Parses the version string and checks if it matched successfully, if not, then an ArgumentException is thrown
            Match match = versionStringRegularExpression.Match( VersionString );

            if ( !match.Success )
            {
                throw new ArgumentException(
                                            $"The version string \"{VersionString}\" is not a valid OpenCL platform version string."
                                           );
            }

            // Saves the version information
            MajorVersion = Convert.ToInt32( match.Groups["MajorVersion"].Value );
            MinorVersion = Convert.ToInt32( match.Groups["MinorVersion"].Value );
            PlatformSpecificInformation = match.Groups["PlatformSpecificInformation"].Value;
        }

        #endregion

    }

}
